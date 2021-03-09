using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Server;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders
{
    public class HubContextProxyMockBuilder : BaseMockBuilder<HubContextProxyMockBuilder, IHubContextProxy>
    {
        public HubContextProxyMockBuilder Where_PublishDtoAsync_returns<TDto>(Response response)
        {
            Mock.Setup(x => x.PublishDtoAsync(It.IsAny<string>(), It.IsAny<TDto>()))
                .ReturnsAsync(response);
            return this;
        }
    }
}