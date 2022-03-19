using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.AccessToken;
using Blauhaus.Common.ValueObjects.BuildConfigs;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.DeviceServices.Abstractions.DeviceInfo;
using Blauhaus.SignalR.Client.Ioc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blauhaus.SignalR.Client.Connection.Proxy
{
    public class SignalRConnectionProxy : ISignalRConnectionProxy
    {
        private readonly IAnalyticsLogger<ISignalRConnectionProxy> _logger;
        private readonly HubConnection _hub;

        public SignalRConnectionProxy(
            IAnalyticsLogger<ISignalRConnectionProxy> logger,
            IServiceProvider serviceProvider,
            ISignalRClientConfig config,
            IAuthenticatedAccessToken accessToken,
            IDeviceInfoService deviceInfoService)
        {
            _logger = logger;

            var builder = new HubConnectionBuilder();

            if (config.IsAutoReconnectEnabled)
            {
                builder.WithAutomaticReconnect();
            }

            var hubUrl = $"{config.HubUrl}?device={deviceInfoService.DeviceUniqueIdentifier}";
            _logger.LogDebug("Constructing SignalR hub connection proxy for url {ApiEndpoint}", hubUrl);

            var loggingProviders = serviceProvider.GetServices<ILoggerProvider>();
            
            builder.ConfigureLogging(logging =>
            {
                foreach (var provider in loggingProviders)
                {
                    _logger.LogTrace("Adding Custom logging provider {LoggingProvider}", provider.GetType().Name);
                    logging.AddProvider(provider);
                }
            });
 
            builder.WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(accessToken.Token);

                if (config.BypassSSLErrors)
                {
                    //https://github.com/xamarin/xamarin-android/issues/6351
                    _logger.LogTrace("SSL errors will be bypassed... ");

                    options.HttpMessageHandlerFactory = (message) =>
                    {
                        if (message is HttpClientHandler clientHandler)
                            // bypass SSL certificate
                            clientHandler.ServerCertificateCustomValidationCallback +=
                                (sender, certificate, chain, sslPolicyErrors) => true;
                        return message;
                    };
                }
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
                _logger.LogDebug("SignalR client is {ConnectionState} so cannot call server. Reconnecting...", _hub.State);
                await ConnectAsync();
            }
            return await _hub.InvokeAsync<TDto>(methodName, parameter);
        }

        public async Task<TDto> InvokeAsync<TDto>(string methodName, object parameter1, object parameter2)
        {
            if (_hub.State != HubConnectionState.Connected)
            {
                _logger.LogDebug("SignalR client is {ConnectionState} so cannot call server. Reconnecting...", _hub.State);
                await ConnectAsync();
            } 
            
            return await _hub.InvokeAsync<TDto>(methodName, parameter1, parameter2);
        }

        public async Task InvokeAsync(string methodName, object parameter)
        {
            if (_hub.State != HubConnectionState.Connected)
            {
                _logger.LogDebug("SignalR client is {ConnectionState} so cannot call server. Reconnecting...", _hub.State);
                await ConnectAsync();
            }
            await _hub.InvokeAsync(methodName, parameter);
        }

        public async Task InvokeAsync(string methodName, object parameter1, object parameter2)
        {
            if (_hub.State != HubConnectionState.Connected)
            {
                _logger.LogDebug("SignalR client is {ConnectionState} so cannot call server. Reconnecting...", _hub.State);
                await ConnectAsync();
            }
            await  _hub.InvokeAsync(methodName, parameter1, parameter2);
        }

        public IDisposable Subscribe<TDto>(string methodName, Func<TDto, Task> handler)
        {
            _logger.LogDebug("SignalR client is {ConnectionState} so cannot call server. Reconnecting...", _hub.State);
            return _hub.On<TDto>(methodName, async dto =>
            {
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
            _logger.LogDebug("SignalR client disposing");
            _hub.Reconnecting -= OnReconnecting;
            _hub.Reconnected -= OnReconnected;
            _hub.Closed -= OnClosed;
            return _hub.DisposeAsync();
        }
    }

  
}