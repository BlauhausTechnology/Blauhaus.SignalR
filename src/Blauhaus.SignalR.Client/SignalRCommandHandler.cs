using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Client._Ioc;
using Blauhaus.SignalR.Client.Extensions;

namespace Blauhaus.SignalR.Client
{
    public class SignalRCommandHandler : ISignalRCommandHandler
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
        }
        
        public async Task<Response> HandleAsync<TCommand>(TCommand command, CancellationToken token = default) where TCommand : notnull
        {
            if (!_connectivityService.IsConnectedToInternet)
            {
                _analyticsService.TraceWarning(this, "SignalR hub could not be invoked because there is no internet connection");
                return Response.Failure(SignalRErrors.NoInternet);
            }
            try
            {
                return await _connectionProxy.InvokeAsync<Response>($"Handle{typeof(TCommand).Name}Async", command, _analyticsService.AnalyticsOperationHeaders, token);
            }
            catch (Exception e)
            {
                return _analyticsService.LogExceptionResponse(this, e, SignalRErrors.InvocationFailure(e), 
                    command.ToObjectDictionary("Command"));
            }
        }
        public async Task<Response<TDto>> HandleAsync<TDto, TCommand>(TCommand command, CancellationToken token = default) where TCommand : notnull
        {
            if (!_connectivityService.IsConnectedToInternet)
            {
                _analyticsService.TraceWarning(this, "SignalR hub could not be invoked because there is no internet connection");
                return Response.Failure<TDto>(SignalRErrors.NoInternet);
            }

            try
            {
                return await _connectionProxy.InvokeAsync<Response<TDto>>($"Handle{typeof(TCommand).Name}Async", command, _analyticsService.AnalyticsOperationHeaders, token);
            }
            catch (Exception e)
            {
                return _analyticsService.LogExceptionResponse<TDto>(this, e, SignalRErrors.InvocationFailure(e), 
                    command.ToObjectDictionary("Command"));
            }
        }

        //todo tests
        public IObservable<SignalRConnectionState> Monitor()
        {
            return Observable.Create<SignalRConnectionState>(observer =>
            {
                var subscriptions = new CompositeDisposable();

                observer.OnNext(_connectionProxy.CurrentState.ToConnectionState());

                void OnHubStateChanged(object sender, ClientConnectionStateChangeEventArgs eventArgs)
                {

                    var clientState = eventArgs.State.ToConnectionState();
                    var exception = eventArgs.Exception;
                    var traceMessage = $"SignalR client hub {clientState}";

                    observer.OnNext(clientState);

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

                }

                subscriptions.Add(Observable.FromEventPattern(
                    x => _connectionProxy.StateChanged += OnHubStateChanged,
                    x => _connectionProxy.StateChanged -= OnHubStateChanged).Subscribe());

                return subscriptions;
            });
        }

        public IObservable<TDto> Connect<TDto>()
        {
            return _connectionProxy.On<TDto>("Publish");
        }


    }
}