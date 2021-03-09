using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Common.Abstractions;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface IDtoCache<TDto> where TDto: class
    {
        Task SaveAsync(TDto dto);
        Task<IReadOnlyList<TDto>> GetAllAsync();
        Task<IReadOnlyList<TDto>> GetWhereAsync(Func<TDto, bool> search);
    }

    public interface IDtoCache<TDto, in TId> : IDtoCache<TDto> 
        where TDto : class, IHasId<TId>
    {
        Task<TDto?> GetOneAsync(TId id);
    }
}