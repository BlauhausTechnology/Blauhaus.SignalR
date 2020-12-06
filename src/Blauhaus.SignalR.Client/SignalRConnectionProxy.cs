using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.AccessToken;
using Blauhaus.DeviceServices.Abstractions.DeviceInfo;
using Blauhaus.Responses;
using Blauhaus.SignalR.Client._Ioc;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.SignalR.Client
{
    public class SignalRConnectionProxy : ISignalRConnectionProxy
    {
        private readonly IDeviceInfoService _deviceInfoService;
        private readonly IAnalyticsService _analyticsService;
        private readonly HubConnection _hub;

        public SignalRConnectionProxy(
            ISignalRClientConfig config,
            IAuthenticatedAccessToken accessToken,
            IDeviceInfoService deviceInfoService,
            IAnalyticsService analyticsService)
        {
            _deviceInfoService = deviceInfoService;
            _analyticsService = analyticsService;

            var builder = new HubConnectionBuilder();
                //.WithAutomaticReconnect();

            var hubUrl = $"{config.HubUrl}?device={deviceInfoService.DeviceUniqueIdentifier}";
            _analyticsService.Trace(this, $"Constructing SignalR hub connection proxy for url {hubUrl}");
 
            builder.WithUrl(hubUrl, options => options.AccessTokenProvider = () =>
            {
                return Task.FromResult(accessToken.Token);
            });
            
            _hub = builder.Build();

            _hub.Reconnecting += OnReconnecting;
            _hub.Reconnected += OnReconnected;
            _hub.Closed += OnClosed;
        }

        private async Task ConnectAsync()
        {
            await _hub.StartAsync();
        }

        public async Task<TDto> InvokeAsync<TDto>(string methodName, object parameter, CancellationToken token = default)
        {
            if (_hub.State != HubConnectionState.Connected)
            {
                _analyticsService.TraceWarning(this, $"SignalR client is {_hub.State} so cannot call server. Reconnecting...");
                await ConnectAsync();
            }
            return await _hub.InvokeAsync<TDto>(methodName, parameter, token);
        }

        public async Task<TDto> InvokeAsync<TDto>(string methodName, object parameter1, object parameter2, CancellationToken token = default)
        {
            if (_hub.State != HubConnectionState.Connected)
            {
                _analyticsService.TraceWarning(this, $"SignalR client is {_hub.State} so cannot call server. Reconnecting...");
                await ConnectAsync();
            }
            return await _hub.InvokeAsync<TDto>(methodName, parameter1, parameter2, token);
        }

        public async Task InvokeAsync(string methodName, object parameter, CancellationToken token = default)
        {
            if (_hub.State != HubConnectionState.Connected)
            {
                _analyticsService.TraceWarning(this, $"SignalR client is {_hub.State} so cannot call server. Reconnecting...");
                await ConnectAsync();
            }
            await _hub.InvokeAsync(methodName, parameter, token);
        }

        public async Task InvokeAsync(string methodName, object parameter1, object parameter2, CancellationToken token = default)
        {
            if (_hub.State != HubConnectionState.Connected)
            {
                _analyticsService.TraceWarning(this, $"SignalR client is {_hub.State} so cannot call server. Reconnecting...");
                await ConnectAsync();
            }
            await  _hub.InvokeAsync(methodName, parameter1, parameter2, token);
        }

        public IObservable<TDto> On<TDto>(string methodName)
        {
            return Observable.Create<TDto>(observer =>
            {
                try
                {
                    return _hub.On<TDto>(methodName, observer.OnNext);
                }
                catch (Exception e)
                {
                    observer.OnError(e);
                    return Disposable.Empty;
                }
            });
        }

        public IDisposable On<TDto>(string methodName, Action<TDto> handler) => _hub.On(methodName, handler);

        public event EventHandler<ClientConnectionStateChangeEventArgs>? StateChanged;
        private Task OnClosed(Exception e)
        {
            StateChanged?.Invoke(this, new ClientConnectionStateChangeEventArgs(HubConnectionState.Disconnected, e));
            return Task.CompletedTask;
        }

        private Task OnReconnected(string arg)
        {
            StateChanged?.Invoke(this, new ClientConnectionStateChangeEventArgs(HubConnectionState.Connected, null));
            return Task.CompletedTask;
        }

        private Task OnReconnecting(Exception e)
        {
            StateChanged?.Invoke(this, new ClientConnectionStateChangeEventArgs(HubConnectionState.Reconnecting, e));
            return Task.CompletedTask;
        }

        public HubConnectionState CurrentState => _hub.State;
        public string ConnectionId => _hub.ConnectionId;
        public Task StopAsync(CancellationToken token) => _hub.StopAsync(token);
        public ValueTask DisposeAsync()
        {
            _hub.Reconnecting -= OnReconnecting;
            _hub.Reconnected -= OnReconnected;
            _hub.Closed -= OnClosed;
            return _hub.DisposeAsync();
        }
    }

  
}