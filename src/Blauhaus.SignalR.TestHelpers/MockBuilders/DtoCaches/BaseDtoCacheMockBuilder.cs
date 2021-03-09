using Blauhaus.Common.Abstractions;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.DtoCaches
{
    public abstract class BaseDtoCacheMockBuilder<TBuilder, TMock, TDto, TId> : BaseMockBuilder<TBuilder, TMock>
        where TBuilder : BaseDtoCacheMockBuilder<TBuilder, TMock, TDto, TId> 
        where TMock : class, IDtoCache<TDto, TId>
        where TDto : class, IHasId<TId>
    {
        public void VerifySaveAsync(TDto dto, int times = 1)
        {
            Mock.Verify(x => x.SaveAsync(dto), Times.Exactly(times));
        }
    }
}