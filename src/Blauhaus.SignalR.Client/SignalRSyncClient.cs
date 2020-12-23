using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.Sync.Abstractions;

namespace Blauhaus.SignalR.Client
{
    public class SignalRSyncClient<TDto> : SignalRClient<TDto>, ISignalRSyncClient<TDto> where TDto : IClientEntity
    {
        private IDisposable? _connectionSubscription;

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
        
        
        public async Task<Response<IDisposable>> SyncAsync(SyncCommand command, Func<TDto, Task> handler)  
        {
            //todo figure out how to handle connection state changes

            try
            {
                var token = await SubscribeAsync(handler);

                if (_connectionSubscription == null)
                {
                    _connectionSubscription = Connection.Subscribe<TDto>($"Publish{typeof(TDto).Name}", async dto =>
                    {
                        await DtoCache.SaveAsync(dto);
                        await UpdateSubscribersAsync(dto);
                    });
                    var syncResult = await Connection.InvokeAsync<Response<SyncResult<TDto>>>($"Sync{typeof(TDto).Name}Async", command, AnalyticsService.AnalyticsOperationHeaders);
                    if (syncResult.IsFailure)
                    {
                        return Response.Failure<IDisposable>(syncResult.Error);
                    }

                    await _syncDtoCache.SaveDtosAsync(syncResult.Value);
                    foreach (var dto in syncResult.Value.EntityBatch)
                    {
                        await UpdateSubscribersAsync(dto);
                    }
                }

                return Response.Success(token);
            }
            catch (ErrorException errorException)
            {
                return AnalyticsService.TraceErrorResponse<IDisposable>(this, errorException.Error, command.ToObjectDictionary());
            }
            catch (Exception e)
            {
                return AnalyticsService.LogExceptionResponse<IDisposable>(this, e, SignalRErrors.InvocationFailure(e), command.ToObjectDictionary());
            }
        }
    }
}