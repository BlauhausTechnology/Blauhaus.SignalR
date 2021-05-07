using Blauhaus.Common.Abstractions;
using Blauhaus.SignalR.Abstractions.Client;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.DtoSavers
{
    public class DtoSaverMockBuilder<TDto, TId> : BaseDtoSaverMockBuilder<DtoSaverMockBuilder<TDto, TId>, IDtoHandler<TDto>, TDto, TId> where TDto : class, IHasId<TId>
    {
        
    }
}