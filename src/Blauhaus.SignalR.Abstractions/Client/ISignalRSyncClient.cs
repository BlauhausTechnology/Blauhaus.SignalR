using System;
using System.Threading.Tasks;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.Responses;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRSyncClient<TDto> : ISignalRClient<TDto, Guid> 
        where TDto : IClientEntity
    {
        
        Task<Response<IDisposable>> SyncAsync(Func<TDto, Task> handler); 

    }
}