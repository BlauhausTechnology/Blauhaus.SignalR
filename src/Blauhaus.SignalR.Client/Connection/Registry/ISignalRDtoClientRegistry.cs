using System.Threading.Tasks;

namespace Blauhaus.SignalR.Client.Connection.Registry
{



    public interface ISignalRDtoClientRegistry
    {
        Task InitializeAllClientsAsync();
    }
}