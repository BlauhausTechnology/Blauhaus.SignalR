using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.Utils.Contracts;
using Blauhaus.Common.Utils.Disposables;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;

namespace Blauhaus.SignalR.Client
{
    
    public class SignalRClient<TDto> : BasePublisher, ISignalRClient<TDto> where TDto : IHasId<Guid>
    {
        protected readonly SemaphoreSlim Locker = new SemaphoreSlim(1); 
        protected readonly ISignalRConnectionProxy Connection;
        
        protected readonly IDtoCache<TDto> DtoCache;
        protected readonly IAnalyticsService AnalyticsService;
        protected readonly IConnectivityService ConnectivityService;

        private IDisposable? _connectToken;

        public SignalRClient(
            IAnalyticsService analyticsService,
            IConnectivityService connectivityService,
            IDtoCache<TDto> dtoCache,
            ISignalRConnectionProxy connection)
        {
            DtoCache = dtoCache;
            Connection = connection;
            AnalyticsService = analyticsService;
            ConnectivityService = connectivityService;
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
                    var dto = result.Value;
                    await UpdateSubscribersAsync(dto);
                    await DtoCache.SaveAsync(dto);
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
        
        public async Task<Response> HandleVoidCommandAsync<TCommand>(TCommand command) where TCommand : notnull
        {
            if (!ConnectivityService.IsConnectedToInternet)
            {
                AnalyticsService.TraceWarning(this, "SignalR hub could not be invoked because there is no internet connection");
                return Response.Failure(SignalRErrors.NoInternet);
            }
            
            await Locker.WaitAsync();
            try
            {
                return await Connection.InvokeAsync<Response>($"HandleVoid{typeof(TCommand).Name}Async", command, AnalyticsService.AnalyticsOperationHeaders);

            }
            catch (ErrorException errorException)
            {
                return AnalyticsService.TraceErrorResponse(this, errorException.Error, command.ToObjectDictionary());
            }
            catch (Exception e)
            {
                return AnalyticsService.LogExceptionResponse(this, e, SignalRErrors.InvocationFailure(e));
            }
            finally
            {
                Locker.Release();
            }
        }
        
        public async Task<Response<IDisposable>> ConnectAsync(Guid id, Func<TDto, Task> handler)
        {
            try
            {
                var token = await SubscribeAsync(handler);

                if (_connectToken == null)
                {
                    AnalyticsService.Trace(this, $"No connections yet for {typeof(TDto).Name}, subscribing for updates from connection");

                    _connectToken  = Connection.Subscribe<TDto>($"Connect{typeof(TDto).Name}Async", async dto =>
                    {
                        if (dto.Id == id)
                        {
                            await DtoCache.SaveAsync(dto);
                            await UpdateSubscribersAsync(dto);
                        }
                    });
                }
                
                return Response.Success(token);
            } 
            catch (Exception e)
            {
                return HandleException<IDisposable>(e);
            }
        }
        
        protected Response<T> HandleException<T>(Exception e)
        {
            if (e is ErrorException errorException)
            {
                return AnalyticsService.TraceErrorResponse<T>(this, errorException.Error);
            }

            return AnalyticsService.LogExceptionResponse<T>(this, e, SignalRErrors.InvocationFailure(e));
        }
         
    }
}