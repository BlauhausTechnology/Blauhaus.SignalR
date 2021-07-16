using Blauhaus.Common.Abstractions;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Abstractions.DtoCaches;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.DtoCaches
{
    public class DtoCacheMockBuilder<TDto, TId> : BaseDtoCacheMockBuilder<DtoCacheMockBuilder<TDto, TId>, IDtoCache<TDto, TId>,TDto, TId> 
        where TDto : class, IHasId<TId>
    {

    }
}