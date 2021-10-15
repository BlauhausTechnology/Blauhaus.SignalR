using Blauhaus.SignalR.Abstractions.Auth;
using Blauhaus.SignalR.Server.Auth;
using Blauhaus.TestHelpers.MockBuilders;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders
{
    public class ConnectedUserFactoryMockBuilder : BaseMockBuilder<ConnectedUserFactoryMockBuilder, IConnectedUserFactory>
    {
        public ConnectedUserFactoryMockBuilder()
        {
            Where_ExtractFromHubContext_returns(ConnectedUserMockBuilder.Default);
        }

        public ConnectedUserFactoryMockBuilder Where_ExtractFromHubContext_returns(IConnectedUser user)
        {
            Mock.Setup(x => x.ExtractFromHubContext(It.IsAny<HubCallerContext>()))
                .Returns(user);
            return this;
        }
    }
}