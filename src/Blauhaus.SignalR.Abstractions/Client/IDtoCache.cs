using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Common.Abstractions;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface IDtoCache<TDto, in TId> 
        where TDto : class, IHasId<TId>
    {
        Task<TDto?> GetOneAsync(TId id);
        Task SaveAsync(TDto dto);
        Task<IReadOnlyList<TDto>> GetAllAsync();
        Task<IReadOnlyList<TDto>> GetWhereAsync(Func<TDto, bool> search);
    }
}