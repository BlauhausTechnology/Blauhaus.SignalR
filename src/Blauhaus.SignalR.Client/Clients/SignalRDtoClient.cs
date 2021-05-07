using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.ClientActors.Actors;
using Blauhaus.Common.Abstractions;
using Blauhaus.Common.Utils.Disposables;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Client.Connection;
using Blauhaus.SignalR.Client.Connection.Proxy;

namespace Blauhaus.SignalR.Client.Clients
{
    
    public class SignalRDtoClient<TDto, TId> : BaseActor, ISignalRDtoClient<TDto>
        where TDto : class, IHasId<TId>
    {
        protected readonly SemaphoreSlim Locker = new SemaphoreSlim(1); 
        protected readonly ISignalRConnectionProxy Connection;
        
        protected readonly IAnalyticsService AnalyticsService;
        protected readonly IConnectivityService ConnectivityService;
        private readonly IEnumerable<Func<TId, Task<IDtoHandler<TDto>>>> _dtoHandlerResolver;

        private IDisposable? _connectToken;

        public SignalRDtoClient(
            IAnalyticsService analyticsService,
            IConnectivityService connectivityService,
            IEnumerable<Func<TId, Task<IDtoHandler<TDto>>>> dtoHandlerResolver,
            ISignalRConnectionProxy connection)
        {
            Connection = connection;
            AnalyticsService = analyticsService;
            ConnectivityService = connectivityService;
            _dtoHandlerResolver = dtoHandlerResolver;
        }
        
        public Task InitializeAsync()
        {
            return InvokeAsync(() =>
            {
                var methodName = $"Publish{typeof(TDto).Name}Async";
                _connectToken ??= Connection.Subscribe<TDto>(methodName, async dto =>
                {
                    AnalyticsService.Debug($"Received {typeof(TDto).Name}");
                    await SaveDtoAsync(dto);
                });
                
                AnalyticsService.Debug($"Initialized SignalR Dto Client for {typeof(TDto).Name} as {methodName}");
            });
        }


        private async Task SaveDtoAsync(TDto dto)
        {
            foreach (var dtoHandlerResolver in _dtoHandlerResolver)
            {
                var handler = await dtoHandlerResolver.Invoke(dto.Id);
                await handler.HandleAsync(dto);
            }
        }
           
        public async Task<Response<TDto>> HandleCommandAsync<TCommand>(TCommand command) where TCommand : notnull
        { 
            if (!ConnectivityService.IsConnectedToInternet)
            {
                AnalyticsService.TraceWarning(this, "SignalR hub could not be invoked because there is no internet connection");
                return Response.Failure<TDto>(SignalRErrors.NoInternet);
            }
            
            await Locker.WaitAsync();
            try
            {
                var result = await Connection.InvokeAsync<Response<TDto>>($"Handle{typeof(TCommand).Name}Async", command, AnalyticsService.AnalyticsOperationHeaders);

                if (result.IsSuccess)
                {
                    AnalyticsService.Debug($"Successfully handled {typeof(TCommand).Name} and received {typeof(TDto).Name} result");

                    await SaveDtoAsync(result.Value);
                }

                return result;
            }
            catch (ErrorException errorException)
            {
                return AnalyticsService.TraceErrorResponse<TDto>(this, errorException.Error, command.ToObjectDictionary());
            }
            catch (Exception e)
            {
                return AnalyticsService.LogExceptionResponse<TDto>(this, e, SignalRErrors.InvocationFailure(e), command.ToObjectDictionary());
            }
            finally
            {
                Locker.Release();
            }
        }
          
    }
}