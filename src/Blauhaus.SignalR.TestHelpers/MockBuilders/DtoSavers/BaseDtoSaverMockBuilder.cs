using Blauhaus.Common.Abstractions;
using Blauhaus.Common.TestHelpers.MockBuilders;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.TestHelpers.MockBuilders.DtoCaches;
using Blauhaus.TestHelpers.MockBuilders;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.DtoSavers
{
    public class BaseDtoSaverMockBuilder<TBuilder, TMock, TDto, TId> : BaseMockBuilder<TBuilder, TMock>
        where TBuilder : BaseDtoSaverMockBuilder<TBuilder, TMock, TDto, TId> 
        where TMock : class, IDtoSaver<TDto>
        where TDto : class, IHasId<TId>
    {
        
    }
}