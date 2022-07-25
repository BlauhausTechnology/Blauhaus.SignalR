using System.Threading.Tasks;
using Blauhaus.SignalR.Abstractions.Auth;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders;

public class AccessTokenProviderMockBuilder : BaseMockBuilder<AccessTokenProviderMockBuilder, IAccessTokenProvider>
{
    public AccessTokenProviderMockBuilder Where_GetAccessTokenAsync_returns(string accessToken)
    {
        Mock.Setup(x => x.GetAccessTokenAsync())
            .ReturnsAsync(accessToken);
        return this;
    }
}