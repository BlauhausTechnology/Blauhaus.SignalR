using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.TestHelpers.MockBuilders;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.DtoCaches
{
    public class DtoCacheMockBuilder<TDto> : BaseDtoCacheMockBuilder<DtoCacheMockBuilder<TDto>,IDtoCache<TDto>,TDto>
    {

    }
}