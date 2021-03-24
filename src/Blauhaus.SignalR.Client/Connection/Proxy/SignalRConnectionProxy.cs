using System;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.AccessToken;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.DeviceServices.Abstractions.DeviceInfo;
using Blauhaus.SignalR.Client.Ioc;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.SignalR.Client.Connection.Proxy
{
    public class SignalRConnectionProxy : ISignalRConnectionProxy
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly HubConnection _hub;

        public SignalRConnectionProxy(
            IRuntimePlatform runtimePlatform,
            ISignalRClientConfig config,
            IAuthenticatedAccessToken accessToken,
            IDeviceInfoService deviceInfoService,
            IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;

            var builder = new HubConnectionBuilder();

            if (config.IsAutoReconnectEnabled)
            {
                builder.WithAutomaticReconnect();
            }

            var hubUrl = $"{config.HubUrl}?device={deviceInfoService.DeviceUniqueIdentifier}";
            _analyticsService.Trace(this, $"Constructing SignalR hub connection proxy for url {hubUrl}");
 
            builder.WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(accessToken.Token);
            });

            _hub = builder.Build();
            
            _hub.Reconnecting += OnReconnecting;
            _hub.Reconnected += OnReconnected;
            _hub.Closed += OnClosed;
        }
         
        private async Task ConnectAsync()
        {
            await OnReconnecting(null);
            await _hub.StartAsync();
            await OnReconnected(_hub.ConnectionId);
        }

        public async Task<TDto> InvokeAsync<TDto>(string methodName, object parameter)
        {
            if (_hub.State != HubConnectionState.Connected)
            {
                _analyticsService.TraceWarning(this, $"SignalR client is {_hub.State} so cannot call server. Reconnecting...");
                await ConnectAsync();
            }
            return await _hub.InvokeAsync<TDto>(methodName, parameter);
        }

        public async Task<TDto> InvokeAsync<TDto>(string methodName, object parameter1, object parameter2)
        {
            if (_hub.State != HubConnectionState.Connected)
            {
                _analyticsService.TraceWarning(this, $"SignalR client is {_hub.State} so cannot call server. Reconnecting...");
                await ConnectAsync();
            } 
            
            return await _hub.InvokeAsync<TDto>(methodName, parameter1, parameter2);
        }

        public async Task InvokeAsync(string methodName, object parameter)
        {
            if (_hub.State != HubConnectionState.Connected)
            {
                _analyticsService.TraceWarning(this, $"SignalR client is {_hub.State} so cannot call server. Reconnecting...");
                await ConnectAsync();
            }
            await _hub.InvokeAsync(methodName, parameter);
        }

        public async Task InvokeAsync(string methodName, object parameter1, object parameter2)
        {
            if (_hub.State != HubConnectionState.Connected)
            {
                _analyticsService.TraceWarning(this, $"SignalR client is {_hub.State} so cannot call server. Reconnecting...");
                await ConnectAsync();
            }
            await  _hub.InvokeAsync(methodName, parameter1, parameter2);
        }

        public IDisposable Subscribe<TDto>(string methodName, Func<TDto, Task> handler)
        {
            _analyticsService.Debug($"Subscription added for {methodName} returning {typeof(TDto).Name}");
            return _hub.On<TDto>(methodName, async dto =>
            {
                _analyticsService.Debug($"SignalR Connection Prxy received {typeof(TDto).Name} from {methodName}");
                await handler.Invoke(dto);
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

        private Task OnReconnecting(Exception? e)
        {
            StateChanged?.Invoke(this, new ClientConnectionStateChangeEventArgs(HubConnectionState.Reconnecting, e));
            return Task.CompletedTask;
        }

        public HubConnectionState CurrentState => _hub.State;
        public string ConnectionId => _hub.ConnectionId;
        public Task StopAsync() => _hub.StopAsync(CancellationToken.None);
        public ValueTask DisposeAsync()
        {
            _analyticsService.Trace(this, "SignalR connection disposing...");
            _hub.Reconnecting -= OnReconnecting;
            _hub.Reconnected -= OnReconnected;
            _hub.Closed -= OnClosed;
            return _hub.DisposeAsync();
        }
    }

  
}