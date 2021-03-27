using System;
using System.Threading.Tasks;
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

namespace Blauhaus.SignalR.Client.Connection
{
    public class SignalRClient : BasePublisher, ISignalRClient
    {
        private HubConnectionState _previousState;
        
        private readonly IAnalyticsService _analyticsService;
        private readonly ISignalRConnectionProxy _connectionProxy;
        private readonly ISignalRDtoClientRegistry _signalRDtoClientRegistry;
        private readonly IConnectivityService _connectivityService;

        public SignalRClient(
            IAnalyticsService analyticsService,
            ISignalRConnectionProxy connectionProxy,
            ISignalRDtoClientRegistry signalRDtoClientRegistry,
            IConnectivityService connectivityService)
        {
            _analyticsService = analyticsService;
            _connectionProxy = connectionProxy;
            _signalRDtoClientRegistry = signalRDtoClientRegistry;
            _connectivityService = connectivityService;

            _connectionProxy.StateChanged += OnHubStateChanged;
        }
        

        public Task<IDisposable> SubscribeAsync(Func<SignalRConnectionState, Task> handler, Func<SignalRConnectionState, bool>? filter = null)
        {
            return Task.FromResult(AddSubscriber(handler, filter));
        }
        
        public async Task DisconnectAsync()
        {
            _analyticsService.Trace(this, "SignalR connection disconnecting on request");
            await UpdateSubscribersAsync(SignalRConnectionState.Disconnecting);
            await _connectionProxy.StopAsync();
            await UpdateSubscribersAsync(SignalRConnectionState.Disconnected);
        }

        public Task InitializeAllClientsAsync()
        {
            return _signalRDtoClientRegistry.InitializeAllClientsAsync();
        }

        public async Task<Response> HandleAsync<TCommand>(TCommand command) where TCommand : notnull
        {
            if (!_connectivityService.IsConnectedToInternet)
            {
                _analyticsService.TraceWarning(this, "SignalR hub could not be invoked because there is no internet connection");
                return Response.Failure(SignalRErrors.NoInternet);
            }
            try
            {
                return await _connectionProxy.InvokeAsync<Response>($"Handle{typeof(TCommand).Name}Async", command, _analyticsService.AnalyticsOperationHeaders);
            }
            catch (Exception e)
            {
                return _analyticsService.LogExceptionResponse(this, e, SignalRErrors.InvocationFailure(e), 
                    command.ToObjectDictionary("Command"));
            }
        }
        
        private async void OnHubStateChanged(object sender, ClientConnectionStateChangeEventArgs eventArgs)
        {
            var clientState = eventArgs.State.ToConnectionState(_previousState);
            var exception = eventArgs.Exception;
             
            var traceMessage = $"SignalR client hub {clientState}";
            
            if (clientState == SignalRConnectionState.Reconnecting && exception != null)
            {
                _analyticsService.TraceWarning(this, $"{traceMessage} due to exception: {exception.Message}");
            }

            else if (clientState == SignalRConnectionState.Disconnected)
            {
                _analyticsService.Trace(this, traceMessage);
                if (exception != null)
                {
                    _analyticsService.TraceWarning(this, $"{traceMessage} due to exception: {exception.Message}");
                }
            }
            else
            {
                _analyticsService.Trace(this, traceMessage);
            }

            await UpdateSubscribersAsync(clientState);

            _previousState = eventArgs.State;
        }

    }
}