using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.SignalR.Abstractions.Client;

namespace Blauhaus.SignalR.Client.DtoCaches
{
    public class InMemoryDtoCache<TDto> : IDtoCache<TDto> where TDto : class
    {
        public Task SaveAsync(TDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<TDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TDto?> GetOneAsync(Func<TDto, bool> search)
        {
            return Task.FromResult<TDto?>(null);
        }

        public Task<IReadOnlyList<TDto>> GetWhereAsync(Func<TDto, bool> search)
        {
            throw new NotImplementedException();
        }
    }
}