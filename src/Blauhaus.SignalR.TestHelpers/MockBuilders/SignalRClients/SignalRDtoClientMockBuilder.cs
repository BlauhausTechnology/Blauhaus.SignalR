using Blauhaus.SignalR.Abstractions.Client;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.SignalRClients
{
    public class SignalRDtoClientMockBuilder<TDto> : BaseSignalRDtoClientMockBuilder<SignalRDtoClientMockBuilder<TDto>, ISignalRDtoClient<TDto>, TDto> where TDto : class
    {
        
    }
}