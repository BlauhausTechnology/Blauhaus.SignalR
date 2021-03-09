using System;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.Utils.Disposables;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Client.Extensions;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.SignalR.Client.Connection
{
    public class SignalRConnection : BasePublisher, ISignalRConnection
    {
        private HubConnectionState _previousState;
        
        private readonly IAnalyticsService _analyticsService;
        private readonly ISignalRConnectionProxy _connectionProxy;

        public SignalRConnection(
            IAnalyticsService analyticsService,
            ISignalRConnectionProxy connectionProxy)
        {
            _analyticsService = analyticsService;
            _connectionProxy = connectionProxy;
            
            _connectionProxy.StateChanged += OnHubStateChanged;
        }
        
        public Task<IDisposable> SubscribeAsync(Func<SignalRConnectionState, Task> handler, Func<SignalRConnectionState, bool>? predicate = null)
        {
            return SubscribeAsync(handler, () => Task.FromResult(_connectionProxy.CurrentState.ToConnectionState(_previousState)));
        }

        public async Task DisconnectAsync()
        {
            _analyticsService.Trace(this, "SignalR connection disconnecting on request");
            await UpdateSubscribersAsync(SignalRConnectionState.Disconnecting);
            await _connectionProxy.StopAsync();
            await UpdateSubscribersAsync(SignalRConnectionState.Disconnected);
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