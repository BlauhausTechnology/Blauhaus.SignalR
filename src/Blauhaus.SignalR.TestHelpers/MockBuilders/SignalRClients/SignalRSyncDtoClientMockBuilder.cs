using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.TestHelpers.MockBuilders;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.SignalRClients
{
    public class SignalRSyncDtoClientMockBuilder<TDto> : BaseSignalRSyncDtoClientMockBuilder<SignalRSyncDtoClientMockBuilder<TDto>, ISignalRSyncDtoClient<TDto>, TDto> 
        where TDto : class
    {
    }

    public abstract class BaseSignalRSyncDtoClientMockBuilder<TBuilder, TMock, TDto> : BaseMockBuilder<TBuilder, TMock>
        where TBuilder : BaseSignalRSyncDtoClientMockBuilder<TBuilder, TMock, TDto> 
        where TDto : class
        where TMock : class, ISignalRSyncDtoClient<TDto>
    {
    }
}