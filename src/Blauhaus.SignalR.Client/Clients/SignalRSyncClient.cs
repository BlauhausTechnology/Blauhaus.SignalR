using System;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Abstractions.Sync;
using Blauhaus.SignalR.Client.Connection;

namespace Blauhaus.SignalR.Client.Clients
{
    public class SignalRSyncClient<TDto> : SignalRClient<TDto>, ISignalRSyncClient<TDto> where TDto : class, IClientEntity
    {
        private IDisposable? _syncToken;

        private readonly ISyncDtoCache<TDto> _syncDtoCache;
        
        public SignalRSyncClient(
            IAnalyticsService analyticsService, 
            IConnectivityService connectivityService, 
            ISyncDtoCache<TDto> dtoCache, 
            ISignalRConnectionProxy connection) 
                : base(analyticsService, connectivityService, dtoCache, connection)
        {
            _syncDtoCache = dtoCache;
        }
        
        
        public async Task<Response<IDisposable>> SyncAsync(Func<TDto, Task> handler)  
        {
            //todo figure out how to handle connection state changes

            try
            {
                var token = await SubscribeAsync(handler);

                if (_syncToken == null)
                {
                    _syncToken = Connection.Subscribe<SyncResponse<TDto>>($"Sync{typeof(TDto).Name}Async", async syncResponse =>
                    {
                        await _syncDtoCache.SaveSyncResponseAsync(syncResponse);
                        foreach (var dto in syncResponse.Dtos)
                        {
                            await UpdateSubscribersAsync(dto);
                        }
                    });

                    var syncRequest = await _syncDtoCache.LoadSyncRequestAsync();
                    var syncResult = await Connection.InvokeAsync<Response<SyncResponse<TDto>>>($"Sync{typeof(TDto).Name}Async", syncRequest, AnalyticsService.AnalyticsOperationHeaders);
                    if (syncResult.IsFailure)
                    {
                        return Response.Failure<IDisposable>(syncResult.Error);
                    }

                    await _syncDtoCache.SaveSyncResponseAsync(syncResult.Value);
                    foreach (var dto in syncResult.Value.Dtos)
                    {
                        await UpdateSubscribersAsync(dto);
                    }
                }

                return Response.Success(token);
            } 
            catch (Exception e)
            {
                return HandleException<IDisposable>(e);
            }
        }
         
        

    }
}