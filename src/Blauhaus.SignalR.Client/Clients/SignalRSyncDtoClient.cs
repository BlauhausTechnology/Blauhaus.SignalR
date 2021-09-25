using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.Domain.Abstractions.DtoCaches;
using Blauhaus.Domain.Abstractions.DtoHandlers;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.Domain.Abstractions.Sync;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Abstractions.Sync;
using Blauhaus.SignalR.Client.Clients.Base;
using Blauhaus.SignalR.Client.Connection.Proxy;
using Newtonsoft.Json;

namespace Blauhaus.SignalR.Client.Clients
{
    public class SignalRSyncDtoClient<TDto, TId> : BaseSignalRDtoClient<TDto,TId, ISyncDtoCache<TDto, TId>>, ISignalRSyncDtoClient<TDto, TId>
        where TDto : class, IClientEntity<TId> 
        where TId : IEquatable<TId>
    { 

        public SignalRSyncDtoClient(
            IAnalyticsService analyticsService, 
            IConnectivityService connectivityService, 
            ISyncDtoCache<TDto, TId> syncDtoCache,
            ISignalRConnectionProxy connection) 
                : base(analyticsService, connectivityService, syncDtoCache, connection)
        {
        }
         
        public async Task<Response<DtoBatch<TDto, TId>>> HandleAsync(DtoSyncCommand command)
        {
            if (!ConnectivityService.IsConnectedToInternet)
            {
                AnalyticsService.TraceWarning(this, "SignalR hub could not be invoked because there is no internet connection");
                return Response.Failure<DtoBatch<TDto, TId>>(SignalRErrors.NoInternet);
            }
            
            await Locker.WaitAsync();
            try
            {
                var result = await Connection.InvokeAsync<Response<DtoBatch<TDto, TId>>>($"Handle{typeof(TDto).Name}SyncCommandAsync", command, AnalyticsService.AnalyticsOperationHeaders);

                if (result.IsFailure)
                {
                    return Response.Failure<DtoBatch<TDto, TId>>(result.Error);
                }
                
                AnalyticsService.Debug($"Successfully handled {nameof(DtoSyncCommand)} and received: {result.Value}" );

                var dtoBatch = result.Value;

                await DtoCache.SaveSyncedDtosAsync(dtoBatch);
                  
                foreach (var dto in dtoBatch.Dtos)
                {
                    await UpdateSubscribersAsync(dto); 
                }

                return Response.Success(dtoBatch);

            }
            catch (ErrorException errorException)
            {
                return AnalyticsService.TraceErrorResponse<DtoBatch<TDto, TId>>(this, errorException.Error, command.ToObjectDictionary());
            }
            catch (Exception e)
            {
                return AnalyticsService.LogExceptionResponse<DtoBatch<TDto, TId>>(this, e, SignalRErrors.InvocationFailure(e), command.ToObjectDictionary());
            }
            finally
            {
                Locker.Release();
            }
        }
    }
}