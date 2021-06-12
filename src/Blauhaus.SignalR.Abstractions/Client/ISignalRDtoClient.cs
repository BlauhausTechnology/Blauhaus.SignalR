using System;
using System.Threading.Tasks;
using Blauhaus.Common.Abstractions;
using Blauhaus.Responses;

namespace Blauhaus.SignalR.Abstractions.Client
{



    public  interface ISignalRDtoClient : IAsyncInitializable
    {

    }

    public interface ISignalRDtoClient<TDto> : ISignalRDtoClient, IAsyncPublisher<TDto>
    {
        
        Task<Response<TDto>> HandleCommandAsync<TCommand>(TCommand command) where TCommand : notnull;
    }
}