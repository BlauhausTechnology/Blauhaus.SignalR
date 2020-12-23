using System.Threading.Tasks;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.SignalR.Abstractions.Sync;
using Blauhaus.Sync.Abstractions;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISyncDtoCache<TDto> : IDtoCache<TDto> where TDto : IClientEntity
    {
        Task SaveDtosAsync(SyncResponse<TDto> syncResult);
    }
}