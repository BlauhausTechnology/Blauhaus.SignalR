using System.Threading.Tasks;
using Blauhaus.Responses;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRClient<TDto>
    {
        Task<Response<TDto>> HandleCommandAsync<TCommand>(TCommand command) where TCommand : notnull;
        Task<Response> HandleVoidCommandAsync<TCommand>(TCommand command) where TCommand : notnull;
    }
}