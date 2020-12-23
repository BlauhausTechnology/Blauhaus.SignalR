using System;
using System.Threading.Tasks;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.Responses;
using Blauhaus.Sync.Abstractions;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRSyncClient<TDto> : ISignalRClient<TDto> 
        where TDto : IClientEntity
    {
        
        Task<Response<IDisposable>> SyncAsync(SyncCommand command, Func<TDto, Task> handler);
    }
}