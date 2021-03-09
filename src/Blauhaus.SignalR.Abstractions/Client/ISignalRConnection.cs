using System.Threading.Tasks;
using Blauhaus.Common.Abstractions;
using Blauhaus.Responses;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRConnection : IAsyncPublisher<SignalRConnectionState>
    {
        Task DisconnectAsync();
        
        Task<Response<TDto>> HandleAsync<TDto, TCommand>(TCommand command) where TCommand : notnull;
        Task<Response> HandleAsync<TCommand>(TCommand command) where TCommand : notnull;
    }
}