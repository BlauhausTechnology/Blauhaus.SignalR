using Blauhaus.SignalR.Abstractions.Client;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.SignalRClients
{
    public class SignalRClientMockBuilder<TDto, TId> : BaseSignalRClientMockBuilder<SignalRClientMockBuilder<TDto, TId>, ISignalRClient<TDto, TId>, TDto, TId> where TDto : class
    {
        
    }
}