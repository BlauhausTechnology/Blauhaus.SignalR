using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Common.Abstractions;
using Blauhaus.Common.Utils.Disposables;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.SignalR.Abstractions.Client;

namespace Blauhaus.SignalR.Client.Sqlite.DtoCache
{
    public abstract class BaseSqliteDtoCache<TDto, TId, TEntity> : BasePublisher, IDtoCache<TDto, TId> 
        where TDto : class, IHasId<TId>
        where TEntity : IClientEntity<TId>
    {

        public Task<IDisposable> SubscribeAsync(Func<TDto, Task> handler, Func<TDto, bool> filter = null)
        {
            return Task.FromResult(AddSubscriber(handler, filter));
        }

        public Task SaveAsync(TDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<TDto> GetOneAsync(TId id)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<TDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<TDto>> GetWhereAsync(Func<TDto, bool> search)
        {
            throw new NotImplementedException();
        }
    }
}