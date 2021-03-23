using System.Threading.Tasks;

namespace Blauhaus.SignalR.Client.ClientRegistry
{
    public interface ISignalRDtoClientRegistry
    {
        Task InitializeAllClientsAsync();
    }
}