using Blauhaus.SignalR.Abstractions.Client;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.SignalRClients
{
    public class SignalRDtoClientMockBuilder<TDto, TId> : BaseSignalRDtoClientMockBuilder<SignalRDtoClientMockBuilder<TDto, TId>, ISignalRDtoClient<TDto>, TDto, TId> where TDto : class
    {
        
    }
}