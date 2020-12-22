using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.Utils.Disposables;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Abstractions.Subscriptions;

namespace Blauhaus.SignalR.Client
{
    
    public class SignalRClient<TDto> : BasePublisher, ISignalRClient<TDto>
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
        
        public async Task<Response<IDisposable>> SubscribeAsync(Func<TDto, Task> handler, DtoSubscription? dtoSubscription = null)
        {
            await _locker.WaitAsync();


            //todo figure out how to handle connection state changes
            
            try
            {
                if (_connectionSubscription == null)
                {
                    _connectionSubscription = _connection.Subscribe<TDto>($"Publish{typeof(TDto).Name}", async dto =>
                    {
                        await _dtoCache.SaveAsync(dto);
                        await UpdateSubscribersAsync(dto);
                    });
                    var subscription = dtoSubscription ?? new DtoSubscription();
                    await _connection.InvokeAsync($"SubscribeTo{typeof(TDto).Name}", subscription, _analyticsService.AnalyticsOperationHeaders);
                }

                var token = await base.SubscribeAsync(handler);
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

        public async Task<Response<TDto>> HandleAsync<TCommand>(TCommand command) where TCommand : notnull
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