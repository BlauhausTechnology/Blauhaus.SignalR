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
                var result = await Connection.InvokeAsync<Response<DtoObjectBatch>>($"Handle{nameof(DtoSyncCommand)}Async", command, AnalyticsService.AnalyticsOperationHeaders);

                if (result.IsSuccess)
                {
                    AnalyticsService.Debug($"Successfully handled {nameof(DtoSyncCommand)} and received: {result.Value}" );

                    var currentBatchCount = result.Value.DtoObjects.Count;
                    var remainingDtoCount = result.Value.RemainingDtoCount;
                    var batchLastModifiedTicks = 0L;

                    var dtos = new TDto[result.Value.DtoObjects.Count];
                    for (var i = 0; i < dtos.Length; i++)
                    {
                        dtos[i] = (TDto)result.Value.DtoObjects[i];
                        if (dtos[i].ModifiedAtTicks > batchLastModifiedTicks)
                        {
                            batchLastModifiedTicks = dtos[i].ModifiedAtTicks;
                        }
                    }
                    foreach (var dto in dtos)
                    {
                        await HandleIncomingDtoAsync(dto);
                    }

                    var dtoBatch = new EmptyDtoBatch<TDto>(currentBatchCount, remainingDtoCount, batchLastModifiedTicks);


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