using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.Sync.Abstractions;

namespace Blauhaus.SignalR.Client
{
    public class SignalRSyncClient<TDto> : SignalRClient<TDto>, ISignalRSyncClient<TDto>
    {
        private IDisposable? _connectionSubscription;

        
        public SignalRSyncClient(
            IAnalyticsService analyticsService, 
            IConnectivityService connectivityService, 
            IDtoCache<TDto> dtoCache, 
            ISignalRConnectionProxy connection) 
                : base(analyticsService, connectivityService, dtoCache, connection)
        {
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

                    foreach (var dto in syncResult.Value.EntityBatch)
                    {
                        await DtoCache.SaveAsync(dto);
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