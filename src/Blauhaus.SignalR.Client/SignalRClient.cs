using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.Utils.Disposables;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;

namespace Blauhaus.SignalR.Client
{
    
    public class SignalRClient<TDto, TSubscribeCommand> : BasePublisher, ISignalRClient<TDto, TSubscribeCommand> where TSubscribeCommand : notnull
    {
        private readonly ISignalRConnectionProxy _connection;
        private readonly IDtoCache<TDto> _dtoCache;
        private readonly IAnalyticsService _analyticsService;
        private readonly IConnectivityService _connectivityService;
        private IDisposable? _connectionSubscription;

        private readonly SemaphoreSlim _locker = new SemaphoreSlim(1); 

        public SignalRClient(
            IAnalyticsService analyticsService,
            IConnectivityService connectivityService,
            IDtoCache<TDto> dtoCache,
            ISignalRConnectionProxy connection)
        {
            _dtoCache = dtoCache;
            _connection = connection;
            _analyticsService = analyticsService;
            _connectivityService = connectivityService;
        }

        public async Task<Response<IDisposable>> SubscribeAsync(TSubscribeCommand command, Func<TDto, Task> handler)  
        {
            await _locker.WaitAsync();


            //todo figure out how to handle connection state changes
            
            try
            {
                var token = await SubscribeAsync(handler);
                
                if (_connectionSubscription == null)
                {
                    _connectionSubscription = _connection.Subscribe<TDto>($"Publish{typeof(TDto).Name}", async dto =>
                    {
                        await _dtoCache.SaveAsync(dto);
                        await UpdateSubscribersAsync(dto);
                    });
                    var subscribeResult = await _connection.InvokeAsync<Response<List<TDto>>>($"SubscribeTo{typeof(TDto).Name}", command, _analyticsService.AnalyticsOperationHeaders);
                    if (subscribeResult.IsFailure)
                    {
                        return Response.Failure<IDisposable>(subscribeResult.Error);
                    }

                    foreach (var dto in subscribeResult.Value)
                    {
                        await _dtoCache.SaveAsync(dto);
                        await UpdateSubscribersAsync(dto);
                    }
                }

                return Response.Success(token);
            }
            catch (Exception e)
            {
                _analyticsService.LogException(this, e);
                throw;
            }
            finally
            {
                _locker.Release();
            }
        }

        public async Task<Response<TDto>> HandleCommandAsync<TCommand>(TCommand command) where TCommand : notnull
        { 
            if (!_connectivityService.IsConnectedToInternet)
            {
                _analyticsService.TraceWarning(this, "SignalR hub could not be invoked because there is no internet connection");
                return Response.Failure<TDto>(SignalRErrors.NoInternet);
            }
            
            await _locker.WaitAsync();
            try
            {
                var result = await _connection.InvokeAsync<Response<TDto>>($"Handle{typeof(TCommand).Name}Async", command, _analyticsService.AnalyticsOperationHeaders);

                if (result.IsSuccess)
                {
                    var dto = result.Value;
                    await UpdateSubscribersAsync(dto);
                    await _dtoCache.SaveAsync(dto);
                }

                return result;
            }
            catch (Exception e)
            {
                return _analyticsService.LogExceptionResponse<TDto>(this, e, SignalRErrors.InvocationFailure(e),
                    command.ToObjectDictionary("Command"));
            }
            finally
            {
                _locker.Release();
            }
        }
    }
}