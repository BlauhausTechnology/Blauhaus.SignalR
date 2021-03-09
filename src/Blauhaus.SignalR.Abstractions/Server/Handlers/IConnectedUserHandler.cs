using System.Threading.Tasks;
using Blauhaus.SignalR.Abstractions.Auth;

namespace Blauhaus.SignalR.Abstractions.Server.Handlers
{
    public interface IConnectedUserHandler
    {
        Task ConnectUserAsync(IConnectedUser user);
        Task DisconnectUserAsync(IConnectedUser user);
    }
}