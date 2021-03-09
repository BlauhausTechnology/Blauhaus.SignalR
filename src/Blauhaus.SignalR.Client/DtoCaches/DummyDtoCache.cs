using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.SignalR.Abstractions.Client;

namespace Blauhaus.SignalR.Client.DtoCaches
{
    public class DummyDtoCache<TDto> : IDtoCache<TDto> where TDto : class
    {
        public Task SaveAsync(TDto dto)
        {
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<TDto>> GetAllAsync()
        {
            return Task.FromResult<IReadOnlyList<TDto>>(new List<TDto>());
        }

        public Task<TDto?> GetOneAsync(Func<TDto, bool> search)
        {
            return Task.FromResult<TDto?>(null);
        }

        public Task<IReadOnlyList<TDto>> GetWhereAsync(Func<TDto, bool> search)
        {
            return Task.FromResult<IReadOnlyList<TDto>>(new List<TDto>());
        }
    }
}