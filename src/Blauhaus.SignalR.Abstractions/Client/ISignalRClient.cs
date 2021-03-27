using System.Threading.Tasks;
using Blauhaus.Common.Abstractions;
using Blauhaus.Responses;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRClient : IAsyncPublisher<SignalRConnectionState>
    {
        Task InitializeAllClientsAsync();
        Task<Response> HandleAsync<TCommand>(TCommand command) where TCommand : notnull;
        Task DisconnectAsync();

    }
}