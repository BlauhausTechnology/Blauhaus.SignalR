using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.DeviceServices.Abstractions.DeviceInfo;
using Blauhaus.SignalR.Abstractions.Auth;
using Blauhaus.SignalR.Client.Ioc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blauhaus.SignalR.Client.Connection.Proxy
{
    public class SignalRConnectionProxy : ISignalRConnectionProxy
    {
        private readonly IAnalyticsLogger<ISignalRConnectionProxy> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISignalRClientConfig _config;
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly IDeviceInfoService _deviceInfoService;
        private HubConnection? _hub;
        private HubConnection Hub => _hub ?? throw new InvalidOperationException("HubConnection must be initialized before use");
        private async Task<HubConnection> GetHubAsync()
        {
            if (_hub == null)
            {
                await InitializeAsync();
            }
            return _hub!;
        }
        public SignalRConnectionProxy(
            IAnalyticsLogger<ISignalRConnectionProxy> logger,
            IServiceProvider serviceProvider,
            ISignalRClientConfig config,
            IAccessTokenProvider accessTokenProvider,
            IDeviceInfoService deviceInfoService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _config = config;
            _accessTokenProvider = accessTokenProvider;
            _deviceInfoService = deviceInfoService;
        }
         
        public async Task InitializeAsync()
        {
            if(_hub is null)
            {
                var builder = new HubConnectionBuilder();

                if (_config.IsAutoReconnectEnabled)
                {
                    builder.WithAutomaticReconnect();
                }

                var deviceId = await _deviceInfoService.GetDeviceIdentifierAsync();
                var hubUrl = $"{_config.HubUrl}?device={deviceId}";
                _logger.LogDebug("Constructing SignalR hub connection proxy for url {ApiEndpoint}", hubUrl);

                var loggingProviders = _serviceProvider.GetServices<ILoggerProvider>();
                
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
                    options.AccessTokenProvider = () => _accessTokenProvider.GetAccessTokenAsync();

                    if (_config.BypassSSLErrors)
                    {
                        if (_deviceInfoService.Platform.Equals(RuntimePlatform.Android))
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
                    }
                });

                _hub = builder.Build();
                
                _hub.Reconnecting += OnReconnecting;
                _hub.Reconnected += OnReconnected;
                _hub.Closed += OnClosed;
            }
            
        }
        
        private async Task ConnectAsync()
        {
            await OnReconnecting(null);
            var hub = await GetHubAsync();
            await hub.StartAsync();
            await OnReconnected(hub.ConnectionId);
        }

        public async Task<TDto> InvokeAsync<TDto>(string methodName, object parameter)
        {
            var hub = await GetHubAsync();
            if (hub.State != HubConnectionState.Connected)
            {
                _logger.LogDebug("SignalR client is {ConnectionState} so cannot invoke {MethodName}. Reconnecting...", hub.State, methodName);
                await ConnectAsync();
            }
            return await hub.InvokeAsync<TDto>(methodName, parameter);
        }

        public async Task<TDto> InvokeAsync<TDto>(string methodName, object parameter1, object parameter2)
        {
            await GetHubAsync();
            if (Hub.State != HubConnectionState.Connected)
            {
                _logger.LogDebug("SignalR client is {ConnectionState} so cannot invoke {MethodName}. Reconnecting...", Hub.State, methodName);
                await ConnectAsync();
            } 
            
            return await Hub.InvokeAsync<TDto>(methodName, parameter1, parameter2);
        }

        public async Task InvokeAsync(string methodName, object parameter)
        {
            await GetHubAsync();
            if (Hub.State != HubConnectionState.Connected)
            {
                _logger.LogDebug("SignalR client is {ConnectionState} so cannot invoke {MethodName}. Reconnecting...", Hub.State, methodName);
                await ConnectAsync();
            }
            await Hub.InvokeAsync(methodName, parameter);
        }

        public async Task InvokeAsync(string methodName, object parameter1, object parameter2)
        {
            await GetHubAsync();
            if (Hub.State != HubConnectionState.Connected)
            {
                _logger.LogDebug("SignalR client is {ConnectionState} so cannot invoke {MethodName}. Reconnecting...", Hub.State, methodName);
                await ConnectAsync();
            }
            await  Hub.InvokeAsync(methodName, parameter1, parameter2);
        }

        public IDisposable Subscribe<TDto>(string methodName, Func<TDto, Task> handler)
        {
            return Hub.On<TDto>(methodName, async (TDto dto) =>
            {
                await handler.Invoke(dto);
            });
        }
         
        public IDisposable On<TDto>(string methodName, Action<TDto> handler) => Hub.On(methodName, handler);

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

        public HubConnectionState CurrentState => Hub.State;
        public string ConnectionId => Hub.ConnectionId;

        public async Task StopAsync()
        {
            _logger.LogDebug("SignalR client stopping");
            await Hub.StopAsync(CancellationToken.None);
        }
        public ValueTask DisposeAsync()
        {
            _logger.LogDebug("SignalR client disposing");
            Hub.Reconnecting -= OnReconnecting;
            Hub.Reconnected -= OnReconnected;
            Hub.Closed -= OnClosed;
            return Hub.DisposeAsync();
        }

    }

  
}