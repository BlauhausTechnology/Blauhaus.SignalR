﻿using System;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Abstractions.Sync;

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
        
        
        public async Task<Response<IDisposable>> SyncAsync(Func<TDto, Task> handler)  
        {
            //todo figure out how to handle connection state changes

            try
            {
                var token = await SubscribeAsync(handler);

                if (_connectionSubscription == null)
                {
                    _connectionSubscription = Connection.Subscribe<SyncResponse<TDto>>($"Update{typeof(TDto).Name}", async syncResponse =>
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
            catch (ErrorException errorException)
            {
                return AnalyticsService.TraceErrorResponse<IDisposable>(this, errorException.Error);
            }
            catch (Exception e)
            {
                return AnalyticsService.LogExceptionResponse<IDisposable>(this, e, SignalRErrors.InvocationFailure(e));
            }
        }
         
    }
}