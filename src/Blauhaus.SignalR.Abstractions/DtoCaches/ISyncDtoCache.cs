using Blauhaus.Domain.Abstractions.Entities;
using System.Threading.Tasks;

namespace Blauhaus.SignalR.Abstractions.DtoCaches
{
    public interface ISyncDtoCache<TDto, in TId> : IDtoCache<TDto, TId> 
        where TDto : class, ISyncClientEntity<TId>
    {
        Task<long> LoadLastModifiedAsync();
    }
}