using Blauhaus.SignalR.Abstractions.Client;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.SignalRClients
{
    public class SignalRClientMockBuilder<TDto> : BaseSignalRClientMockBuilder<SignalRClientMockBuilder<TDto>, ISignalRClient<TDto>, TDto> where TDto : class
    {
        
    }
}