using System;
using System.Threading.Tasks;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Sync;
using Blauhaus.Sync.Abstractions;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRSyncClient<TDto> : ISignalRClient<TDto> 
        where TDto : IClientEntity
    {
        
        Task<Response<IDisposable>> SyncAsync(SyncRequest request, Func<TDto, Task> handler);
    }
}