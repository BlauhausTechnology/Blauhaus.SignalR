using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Common.Abstractions;

namespace Blauhaus.SignalR.Abstractions.Client
{

    public interface IDtoSaver<in TDto>
    {
        Task SaveAsync(TDto dto);
    }

    public interface IDtoCache<TDto, in TId> : IAsyncPublisher<TDto>, IDtoSaver<TDto>
        where TDto : class, IHasId<TId>
    {
        Task<TDto?> GetOneAsync(TId id);
        Task<IReadOnlyList<TDto>> GetAllAsync();
        Task<IReadOnlyList<TDto>> GetWhereAsync(Func<TDto, bool> search);
    }
}