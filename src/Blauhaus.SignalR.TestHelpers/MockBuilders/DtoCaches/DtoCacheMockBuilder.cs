using Blauhaus.SignalR.Abstractions.Client;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.DtoCaches
{
    public class DtoCacheMockBuilder<TDto> : BaseDtoCacheMockBuilder<DtoCacheMockBuilder<TDto>,IDtoCache<TDto>,TDto> where TDto : class
    {

    }
}