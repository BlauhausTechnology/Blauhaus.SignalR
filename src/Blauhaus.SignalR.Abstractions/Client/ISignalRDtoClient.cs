using System;
using System.Threading.Tasks;
using Blauhaus.Common.Abstractions;
using Blauhaus.Responses;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRDtoClient<TDto> : IAsyncInitializable
    {
        
        Task<Response<TDto>> HandleCommandAsync<TCommand>(TCommand command) where TCommand : notnull;
    }
}