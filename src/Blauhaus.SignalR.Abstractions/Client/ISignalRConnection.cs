using System.Threading.Tasks;
using Blauhaus.Common.Utils.Contracts;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRConnection : IAsyncPublisher<SignalRConnectionState>
    {
        Task DisconnectAsync();
    }
}