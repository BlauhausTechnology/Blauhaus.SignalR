using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.TestHelpers.MockBuilders;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders
{
    public class DtoCacheMockBuilder<TDto> :BaseMockBuilder<DtoCacheMockBuilder<TDto>, IDtoCache<TDto>>
    {
        public void VerifySaveAsync(TDto dto)
        {
            Mock.Verify(x => x.SaveAsync(dto));
        }
    }
}