using System;
using System.Text.Json.Serialization;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Errors;
using Blauhaus.Errors.Extensions;
using Blauhaus.SignalR.Abstractions.Auth;

namespace Blauhaus.SignalR.Server.Auth
{
    public class ConnectedUser : AuthenticatedUser, IConnectedUser
    {
        [JsonConstructor]
        public ConnectedUser(
            IAuthenticatedUser authenticatedUser, 
            string currentDeviceIdentifier, 
            string currentConnectionId) 
                : base(authenticatedUser)
        {
            if (authenticatedUser.UserId == Guid.Empty)
                throw new ErrorException(Errors.Errors.RequiredValue<IConnectedUser>(x => x.UserId));

            CurrentDeviceIdentifier = currentDeviceIdentifier.ThrowIfNullOrWhiteSpace<IConnectedUser>(x => x.CurrentDeviceIdentifier);
            CurrentConnectionId = currentConnectionId.ThrowIfNullOrWhiteSpace<IConnectedUser>(x => x.CurrentConnectionId);

        }

        public string CurrentDeviceIdentifier { get; }
        public string CurrentConnectionId { get; }
    }
}