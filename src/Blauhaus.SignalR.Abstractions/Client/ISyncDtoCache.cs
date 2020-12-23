using System.Threading.Tasks;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.Sync.Abstractions;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISyncDtoCache<TDto> : IDtoCache<TDto> where TDto : ISyncClientEntity
    {
        Task SaveDtosAsync(SyncResult<TDto> syncResult);
    }
}