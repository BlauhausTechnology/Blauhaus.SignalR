using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blauhaus.ClientActors.Actors;
using Blauhaus.Common.Abstractions;
using Blauhaus.SignalR.Abstractions.Client;

namespace Blauhaus.SignalR.Client.DtoCache
{
    public class InMemoryDtoCache<TDto, TId> : BaseActor, IDtoCache<TDto, TId> where TDto : class, IHasId<TId>
    {

        protected Dictionary<TId, TDto> CachedDtos = new Dictionary<TId, TDto>();

        public Task HandleAsync(TDto dto)
        {
            return InvokeAsync(async () =>
            {
                CachedDtos[dto.Id] = dto;
                await UpdateSubscribersAsync(dto);
            });
        }

        public Task<IDisposable> SubscribeAsync(Func<TDto, Task> handler, Func<TDto, bool>? filter = null)
        {
            return InvokeAsync(() => AddSubscriber(handler, filter));
        }

        public Task<TDto?> GetOneAsync(TId id)
        {
            return InvokeAsync(() =>
                CachedDtos.TryGetValue(id, out var dto)
                    ? dto
                    : null);
        }

        public Task<IReadOnlyList<TDto>> GetAllAsync()
        {
            return InvokeAsync<IReadOnlyList<TDto>>(() =>
                CachedDtos.Values.ToList());
        }

        public Task<IReadOnlyList<TDto>> GetWhereAsync(Func<TDto, bool> search)
        {
            return InvokeAsync<IReadOnlyList<TDto>>(() =>
                CachedDtos.Values.Where(search).ToList());
        }
    }
}