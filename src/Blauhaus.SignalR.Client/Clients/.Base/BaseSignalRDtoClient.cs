using System;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.ClientActors.Actors;
using Blauhaus.Common.Abstractions;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.Domain.Abstractions.DtoCaches;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Client.Connection.Proxy;
using Microsoft.Extensions.Logging;

namespace Blauhaus.SignalR.Client.Clients.Base
{
    
    public abstract class BaseSignalRDtoClient<TDto, TId, TDtoCache> : BaseActor, ISignalRDtoClient<TDto>
        where TDto : class, IHasId<TId> 
        where TId : IEquatable<TId>
        where TDtoCache : IDtoCache<TDto, TId>
    {
        protected readonly SemaphoreSlim Locker = new(1);
        protected readonly IAnalyticsLogger Logger;
        private readonly IAnalyticsContext _analyticsContext;
        protected readonly ISignalRConnectionProxy Connection;
        
        protected readonly IConnectivityService ConnectivityService;
        protected readonly TDtoCache DtoCache;

        private IDisposable? _connectToken;

        protected BaseSignalRDtoClient(
            IAnalyticsLogger logger,
            IAnalyticsContext analyticsContext,
            IConnectivityService connectivityService,
            TDtoCache dtoCache,
            ISignalRConnectionProxy connection)
        {
            Logger = logger;
            _analyticsContext = analyticsContext;
            Connection = connection;
            ConnectivityService = connectivityService;
            DtoCache = dtoCache;
        }
        
        public async Task InitializeAsync()
        {
            await Connection.InitializeAsync();
            EnsureSubscribedToIncomingDtos();
        }

        private void EnsureSubscribedToIncomingDtos()
        {
            if (_connectToken == null)
            {
                var methodName = $"Publish{typeof(TDto).Name}Async";

                _connectToken = Connection.Subscribe<TDto>(methodName, async dto =>
                {
                    Logger.LogTrace("SignalRDtoClient received {DtoType}", typeof(TDto).Name);
                    await HandleIncomingDtoAsync(dto);
                });
                            
                Logger.LogDebug("Initialized SignalR Dto Client for {DtoType} with callback {Callback}",typeof(TDto).Name, methodName);
            }
        }


        protected virtual async Task HandleIncomingDtoAsync(TDto dto)
        {
            await DtoCache.HandleAsync(dto);
            await UpdateSubscribersAsync(dto); 
        }
           
        public async Task<Response<TDto>> HandleCommandAsync<TCommand>(TCommand command) where TCommand : notnull
        { 
            if (!ConnectivityService.IsConnectedToInternet)
            {
                Logger.LogWarning("SignalR hub could not be invoked because there is no internet connection");
                return Response.Failure<TDto>(SignalRErrors.NoInternet);
            }
            
            await Locker.WaitAsync();
            try
            {
                var result = await Connection.InvokeAsync<Response<TDto>>($"Handle{typeof(TCommand).Name}Async", command, _analyticsContext.GetAllValues());

                if (result.IsSuccess)
                {
                    Logger.LogDebug("Successfully handled {CommandType} and received {DtoType} result", typeof(TCommand).Name, typeof(TDto).Name);
                    await HandleIncomingDtoAsync(result.Value);
                }

                return result;
            }
            catch (ErrorException errorException)
            {
                return Logger.LogErrorResponse<TDto>(errorException.Error);
            }
            catch (Exception e)
            {
                return Logger.LogErrorResponse<TDto>(SignalRErrors.InvocationFailure(e), e);
            }
            finally
            {
                Locker.Release();
            }
        }

        public Task<IDisposable> SubscribeAsync(Func<TDto, Task> handler, Func<TDto, bool>? filter = null)
        {
            return InvokeAsync(() =>
            {
                EnsureSubscribedToIncomingDtos();

                return AddSubscriber(handler, filter);
            });
        }
    }
}