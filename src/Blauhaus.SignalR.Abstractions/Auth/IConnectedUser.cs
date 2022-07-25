using Blauhaus.Auth.Abstractions.User;

namespace Blauhaus.SignalR.Abstractions.Auth
{
    public interface IConnectedUser : IAuthenticatedUser
    {
        string UniqueId { get; }
        string CurrentDeviceIdentifier { get; }
        string CurrentConnectionId { get; }
        string CurrentIpAddress { get; }
    }
}