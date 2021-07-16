using Blauhaus.Common.Abstractions;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Abstractions.DtoCaches;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.DtoSavers
{
    public class DtoHandlerMockBuilder<TDto, TId> : BaseDtoSaverMockBuilder<DtoHandlerMockBuilder<TDto, TId>, IDtoHandler<TDto>, TDto, TId> where TDto : class, IHasId<TId>
    {
        
    }
}