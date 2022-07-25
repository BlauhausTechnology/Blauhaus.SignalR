using Blauhaus.Errors;
using Blauhaus.Responses;
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
            Where_ExtractFromHubContext_succeeds(ConnectedUserMockBuilder.Default);
        }

        public ConnectedUserFactoryMockBuilder Where_ExtractFromHubContext_succeeds(IConnectedUser user)
        {
            Mock.Setup(x => x.ExtractFromHubContext(It.IsAny<HubCallerContext>()))
                .Returns(Response.Success(user));
            return this;
        }
        public Error Where_ExtractFromHubContext_fails(Error? error)
        {
            error ??= Error.Create("ExtractFromHubContext");
            Mock.Setup(x => x.ExtractFromHubContext(It.IsAny<HubCallerContext>()))
                .Returns(Response.Failure<IConnectedUser>(error.Value));
            return error.Value;
        }
    }
}