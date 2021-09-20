using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.Domain.Abstractions.DtoHandlers;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.Domain.Abstractions.Sync;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Client.Connection.Proxy;

namespace Blauhaus.SignalR.Client.Clients
{
    public class SignalRSyncDtoClient<TDto, TId> : SignalRDtoClient<TDto,TId>, ISignalRSyncDtoClient<TDto>
        where TDto : class, IClientEntity<TId> 
        where TId : IEquatable<TId>
    {
        public SignalRSyncDtoClient(
            IAnalyticsService analyticsService, 
            IConnectivityService connectivityService, 
            IEnumerable<Func<TId, Task<IDtoHandler<TDto>>>> dtoHandlerResolver, 
            ISignalRConnectionProxy connection) 
                : base(analyticsService, connectivityService, dtoHandlerResolver, connection)
        {
        }

        public async Task<Response<IDtoBatch<TDto>>> HandleAsync(DtoSyncCommand command)
        {
            if (!ConnectivityService.IsConnectedToInternet)
            {
                AnalyticsService.TraceWarning(this, "SignalR hub could not be invoked because there is no internet connection");
                return Response.Failure<IDtoBatch<TDto>>(SignalRErrors.NoInternet);
            }
            
            await Locker.WaitAsync();
            try
            {
                var result = await Connection.InvokeAsync<Response<IDtoBatch>>($"Handle{nameof(DtoSyncCommand)}Async", command, AnalyticsService.AnalyticsOperationHeaders);

                if (result.IsSuccess)
                {
                    AnalyticsService.Debug($"Successfully handled {nameof(DtoSyncCommand)} and received: {result.Value}" );

                    var dtoBatch = (DtoBatch<TDto, TId>) result.Value;
                    foreach (var dto in dtoBatch.Dtos)
                    {
                        await HandleIncomingDtoAsync(dto);
                    }

                    return Response.Success<IDtoBatch<TDto>>(dtoBatch);
                }

                return Response.Failure<IDtoBatch<TDto>>(result.Error);

            }
            catch (ErrorException errorException)
            {
                return AnalyticsService.TraceErrorResponse<IDtoBatch<TDto>>(this, errorException.Error, command.ToObjectDictionary());
            }
            catch (Exception e)
            {
                return AnalyticsService.LogExceptionResponse<IDtoBatch<TDto>>(this, e, SignalRErrors.InvocationFailure(e), command.ToObjectDictionary());
            }
            finally
            {
                Locker.Release();
            }
        }
    }
}