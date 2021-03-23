using System.Threading.Tasks;

namespace Blauhaus.SignalR.Client.Connection
{
    public interface ISignalRDtoClientRegistry
    {
        Task InitializeAllClientsAsync();
    }
}