using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.DtoCaches
{
    public abstract class BaseDtoCacheMockBuilder<TBuilder, TMock, TDto> : BaseMockBuilder<TBuilder, TMock>
        where TBuilder : BaseDtoCacheMockBuilder<TBuilder, TMock, TDto> 
        where TMock : class, IDtoCache<TDto>
        where TDto : class
    {
        public void VerifySaveAsync(TDto dto, int times = 1)
        {
            Mock.Verify(x => x.SaveAsync(dto), Times.Exactly(times));
        }
    }
}