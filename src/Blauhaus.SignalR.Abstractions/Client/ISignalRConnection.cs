using System.Threading.Tasks;
using Blauhaus.Common.Abstractions;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRConnection : IAsyncPublisher<SignalRConnectionState>
    {
        Task DisconnectAsync();
    }
}