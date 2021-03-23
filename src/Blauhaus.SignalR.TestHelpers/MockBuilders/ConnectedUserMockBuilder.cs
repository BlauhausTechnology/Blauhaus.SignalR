using System;
using System.Collections.Generic;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Common.Utils.Extensions;
using Blauhaus.SignalR.Abstractions.Auth;
using Blauhaus.TestHelpers.MockBuilders;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders
{
    public class ConnectedUserMockBuilder : BaseMockBuilder<ConnectedUserMockBuilder, IConnectedUser>
    {
        public ConnectedUserMockBuilder()
        {
            With(x => x.EmailAddress, Guid.NewGuid() + "@freever.com");
            With(x => x.Claims, new List<UserClaim>());
            With(x => x.CurrentConnectionId, Guid.NewGuid().ToString());
            With(x => x.CurrentDeviceIdentifier, Guid.NewGuid().ToString());
            With(x => x.UserId, Guid.NewGuid());
            Mock.Setup(x => x.UniqueId).Returns(() => $"{Mock.Object.UserId}|{Mock.Object.CurrentDeviceIdentifier}");
        }
        
        public ConnectedUserMockBuilder With_UserId(Guid userId)
        {
            With(x => x.UserId, userId);
            return this;
        }
        
        public ConnectedUserMockBuilder With_DeviceIdentifier(string deviceIdentifier)
        {
            With(x => x.CurrentDeviceIdentifier, deviceIdentifier);
            return this;
        }
        
        public ConnectedUserMockBuilder With_ConnectionId(string connectionId)
        {
            With(x => x.CurrentConnectionId, connectionId);
            return this;
        }
        
        public ConnectedUserMockBuilder With_Claim(UserClaim claim)
        {
            Mock.Setup(x => x.HasClaim(claim.Name)).Returns(true);
            Mock.Setup(x => x.HasClaimValue(claim.Name, claim.Value)).Returns(true);
            With(x => x.Claims, new List<UserClaim>{claim});
            return this;
        }
    }
}