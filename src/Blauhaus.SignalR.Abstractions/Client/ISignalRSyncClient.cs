using System;
using System.Threading.Tasks;
using Blauhaus.Responses;
using Blauhaus.Sync.Abstractions;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRSyncClient<TDto> : ISignalRClient<TDto> 
    {
        
        Task<Response<IDisposable>> SyncAsync(SyncCommand command, Func<TDto, Task> handler);
    }
}