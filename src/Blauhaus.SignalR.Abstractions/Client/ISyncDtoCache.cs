using System;
using System.Threading.Tasks;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.SignalR.Abstractions.Sync;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISyncDtoCache<TDto> : IDtoCache<TDto, Guid> where TDto : class, IClientEntity
    {
        Task SaveSyncResponseAsync(SyncResponse<TDto> syncResult);
        Task<SyncRequest> LoadSyncRequestAsync();
    }
}