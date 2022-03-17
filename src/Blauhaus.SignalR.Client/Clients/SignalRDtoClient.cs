using System;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.Abstractions;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.Domain.Abstractions.DtoCaches;
using Blauhaus.SignalR.Client.Clients.Base;
using Blauhaus.SignalR.Client.Connection.Proxy;

namespace Blauhaus.SignalR.Client.Clients
{

    public class SignalRDtoClient<TDto, TId> : BaseSignalRDtoClient<SignalRDtoClient<TDto, TId>, TDto, TId, IDtoCache<TDto, TId>> where TId : IEquatable<TId> where TDto : class, IHasId<TId>
    {
        public SignalRDtoClient(
            IAnalyticsLogger<SignalRDtoClient<TDto, TId>> logger, 
            IAnalyticsContext analyticsContext,
            IConnectivityService connectivityService, 
            IDtoCache<TDto, TId> dtoCache, 
            ISignalRConnectionProxy connection) 
                : base(logger, analyticsContext, connectivityService, dtoCache, connection)
        {
        }
    }

}