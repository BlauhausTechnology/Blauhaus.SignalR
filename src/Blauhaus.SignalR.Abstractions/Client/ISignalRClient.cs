using System;
using System.Threading.Tasks;
using Blauhaus.Common.Abstractions;
using Blauhaus.Responses;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRClient<TDto, in TId> : IAsyncIdPublisher<TDto, TId>
    {
        void Connect();
        
        Task<Response<TDto>> HandleCommandAsync<TCommand>(TCommand command) where TCommand : notnull;
        Task<Response> HandleVoidCommandAsync<TCommand>(TCommand command) where TCommand : notnull;
    }
}