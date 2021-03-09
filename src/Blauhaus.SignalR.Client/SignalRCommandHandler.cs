using System;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.Utils.Disposables;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Client.Extensions;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.SignalR.Client
{
    public class SignalRCommandHandler : BasePublisher, ISignalRCommandHandler
    {
        private readonly ISignalRConnectionProxy _connectionProxy;
        private readonly IAnalyticsService _analyticsService;
        private readonly IConnectivityService _connectivityService;

        public SignalRCommandHandler(
            ISignalRConnectionProxy connectionProxy,
            IAnalyticsService analyticsService,
            IConnectivityService connectivityService)
        {
            _connectionProxy = connectionProxy;
            _analyticsService = analyticsService;
            _connectivityService = connectivityService;

            _connectionProxy.StateChanged += OnHubStateChanged;
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

        public async Task<Response<TDto>> HandleAsync<TDto, TCommand>(TCommand command) where TCommand : notnull
        {
            if (!_connectivityService.IsConnectedToInternet)
            {
                _analyticsService.TraceWarning(this, "SignalR hub could not be invoked because there is no internet connection");
                return Response.Failure<TDto>(SignalRErrors.NoInternet);
            }

            try
            {
                return await _connectionProxy.InvokeAsync<Response<TDto>>($"Handle{typeof(TCommand).Name}Async", command, _analyticsService.AnalyticsOperationHeaders);
            }
            catch (Exception e)
            {
                return _analyticsService.LogExceptionResponse<TDto>(this, e, SignalRErrors.InvocationFailure(e), 
                    command.ToObjectDictionary("Command"));
            }
        }
        
        //todo tests

        public Task<IDisposable> SubscribeAsync(Func<SignalRConnectionState, Task> handler)
        {
            return SubscribeAsync(handler, () => Task.FromResult(_connectionProxy.CurrentState.ToConnectionState(_previousState)));
        }

        private HubConnectionState _previousState;
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
                _analyticsService.TraceWarning(this, traceMessage);
                if (exception != null)
                {
                    _analyticsService.LogException(this, exception);
                }
            }
            else
            {
                _analyticsService.Trace(this, traceMessage);
            }

            await UpdateSubscribersAsync(clientState);

            _previousState = eventArgs.State;
        }
         
        public IDisposable SubscribeToDto<TDto>(Func<TDto, Task> handler)
        {
            return _connectionProxy.Subscribe($"Publish{typeof(TDto).Name}", handler);
        }

    }
}