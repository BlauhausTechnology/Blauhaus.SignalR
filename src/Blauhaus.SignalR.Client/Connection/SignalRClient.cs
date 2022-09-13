using System;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.Utils.Disposables;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Client.Connection.Proxy;
using Blauhaus.SignalR.Client.Connection.Registry;
using Blauhaus.SignalR.Client.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Blauhaus.SignalR.Client.Connection
{
    public class SignalRClient : BasePublisher, ISignalRClient
    {
        private HubConnectionState _previousState;
        
        private readonly IAnalyticsLogger<SignalRClient> _logger;
        private readonly IAnalyticsContext _analyticsContext;
        private readonly ISignalRConnectionProxy _connectionProxy;
        private readonly ISignalRDtoClientRegistry _signalRDtoClientRegistry;
        private readonly IConnectivityService _connectivityService;
        private readonly Task _connectionTask;

        public SignalRClient(
            IAnalyticsLogger<SignalRClient> logger,
            IAnalyticsContext analyticsContext,
            ISignalRConnectionProxy connectionProxy,
            ISignalRDtoClientRegistry signalRDtoClientRegistry,
            IConnectivityService connectivityService)
        {
            _logger = logger;
            _analyticsContext = analyticsContext;
            _connectionProxy = connectionProxy;
            _signalRDtoClientRegistry = signalRDtoClientRegistry;
            _connectivityService = connectivityService;

            _connectionTask = _connectionProxy.InitializeAsync();
            _connectionProxy.StateChanged += OnHubStateChanged;
        }
        
        public async Task<IDisposable> SubscribeAsync(Func<SignalRConnectionState, Task> handler, Func<SignalRConnectionState, bool>? filter = null)
        {
            if (!_connectionTask.IsCompleted)
            {
                await _connectionTask;
            }
            return AddSubscriber(handler, filter);
        }

        public async Task DisconnectAsync()
        {
            _logger.LogTrace("SignalR connection disconnecting on request");
            await UpdateSubscribersAsync(SignalRConnectionState.Disconnecting);
            await _connectionProxy.StopAsync();
            await UpdateSubscribersAsync(SignalRConnectionState.Disconnected);
        }

        public async Task InitializeAllClientsAsync()
        {
            if (!_connectionTask.IsCompleted)
            {
                await _connectionTask;
            }
            await _signalRDtoClientRegistry.InitializeAllClientsAsync();
        }

        public async Task<Response> HandleVoidCommandAsync<TCommand>(TCommand command) where TCommand : notnull
        {
            if (!_connectivityService.IsConnectedToInternet)
            {
                _logger.LogWarning("SignalR hub could not be invoked because there is no internet connection");
                return Response.Failure(SignalRErrors.NoInternet);
            }
            try
            {
                if (!_connectionTask.IsCompleted) 
                    await _connectionTask;
                
                return await _connectionProxy.InvokeAsync<Response>($"Handle{typeof(TCommand).Name}Async", command, _analyticsContext.GetAllValues());
            }
            catch (Exception e)
            {
                return _logger.LogErrorResponse(SignalRErrors.InvocationFailure(e), e);
            }
        }
        
        public async Task<Response<TResponse>> HandleCommandAsync<TCommand, TResponse>(TCommand command) where TCommand : notnull
        {
            if (!_connectivityService.IsConnectedToInternet)
            {
                _logger.LogWarning("SignalR hub could not be invoked because there is no internet connection");
                return Response.Failure<TResponse>(SignalRErrors.NoInternet);
            }
            try
            {
                if (!_connectionTask.IsCompleted) 
                    await _connectionTask;
                
                return await _connectionProxy.InvokeAsync<Response<TResponse>>($"Handle{typeof(TCommand).Name}Async", command, _analyticsContext.GetAllValues());
            }
            catch (Exception e)
            {
                return _logger.LogErrorResponse<TResponse>(SignalRErrors.InvocationFailure(e), e);
            }
        }

        
        private async void OnHubStateChanged(object sender, ClientConnectionStateChangeEventArgs eventArgs)
        {
            var connectionState = eventArgs.State.ToConnectionState(_previousState);
            var exception = eventArgs.Exception;
             
            
            if (connectionState == SignalRConnectionState.Reconnecting && exception != null)
            {
                _logger.LogWarning("SignalR client hub {ConnectionState} due to exception {@Exception}", connectionState, exception);
            }

            else if (connectionState == SignalRConnectionState.Disconnected)
            {
                _logger.LogTrace("SignalR client hub {ConnectionState}", connectionState);
                if (exception != null)
                {
                    _logger.LogWarning("SignalR client hub {ConnectionState} due to exception {@Exception}", connectionState, exception);
                }
            }
            else
            {
                _logger.LogTrace("SignalR client hub {ConnectionState}", connectionState);
            }

            await UpdateSubscribersAsync(connectionState);

            _previousState = eventArgs.State;
        }

        public async Task InitializeAsync()
        {
            if (!_connectionTask.IsCompleted) 
                await _connectionTask;
        }
    }
}