using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Server;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders
{
    public class HubContextProxyMockBuilder : BaseMockBuilder<HubContextProxyMockBuilder, IHubContextProxy>
    {
        public HubContextProxyMockBuilder Where_SendAsync_returns<TDto>(Response response)
        {
            Mock.Setup(x => x.SendDtoAsync(It.IsAny<string>(), It.IsAny<TDto>()))
                .ReturnsAsync(response);
            return this;
        }
    }
}