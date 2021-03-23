using System.Threading.Tasks;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRDtoClientRegistry
    {
        Task InitializeAllClientsAsync();
    }
}