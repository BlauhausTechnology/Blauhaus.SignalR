using System;
using System.Threading.Tasks;
using Blauhaus.Responses;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRClient<TDto, in TSubscribeCommand> where TSubscribeCommand : notnull
    {
        Task<Response<TDto>> HandleCommandAsync<TCommand>(TCommand command) where TCommand : notnull;
        Task<Response<IDisposable>> SubscribeAsync(TSubscribeCommand command, Func<TDto, Task> handler);
    }
}