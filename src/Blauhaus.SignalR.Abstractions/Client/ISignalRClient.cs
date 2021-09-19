using System.Threading.Tasks;
using Blauhaus.Common.Abstractions;
using Blauhaus.Responses;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRClient : IAsyncPublisher<SignalRConnectionState>, IGenericCommandHandler
    {
        Task InitializeAllClientsAsync();
        Task<Response> HandleAsync<TCommand>(TCommand command) where TCommand : notnull;
        Task<Response<TResponse>> HandleAsync<TCommand, TResponse>(TCommand command) where TCommand : notnull;
        Task DisconnectAsync();

    }

    public interface IGenericCommandHandler
    {
        Task<Response<TResponse>> HandleAsync<TCommand, TResponse>(TCommand command) where TCommand : notnull;
    }

    public interface IGenericCommandHandler<TResponse>
    {
        Task<Response<TResponse>> HandleAsync<TCommand>(TCommand command) where TCommand : notnull;
    }
}