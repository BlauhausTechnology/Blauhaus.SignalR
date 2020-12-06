using System;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Responses;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRCommandHandler
    {
        IObservable<SignalRConnectionState> Monitor();        
        IObservable<TDto> Connect<TDto>();

        Task<Response<TDto>> HandleAsync<TDto, TCommand>(TCommand command, CancellationToken token = default);
        Task<Response> HandleAsync<TCommand>(TCommand command, CancellationToken token = default);

    }
}