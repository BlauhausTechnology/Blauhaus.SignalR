using System;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Common.Utils.Contracts;
using Blauhaus.Responses;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRCommandHandler  : IAsyncPublisher<SignalRConnectionState>
    {
        IDisposable SubscribeToDto<TDto>(Func<TDto, Task> handler);

        Task<Response<TDto>> HandleAsync<TDto, TCommand>(TCommand command) where TCommand : notnull;
        Task<Response> HandleAsync<TCommand>(TCommand command) where TCommand : notnull;

    }
}