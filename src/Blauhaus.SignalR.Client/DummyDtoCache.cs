using System.Threading.Tasks;
using Blauhaus.SignalR.Abstractions.Client;

namespace Blauhaus.SignalR.Client
{
    public class DummyDtoCache<TDto> : IDtoCache<TDto>
    {
        public Task SaveAsync(TDto dto)
        {
            return Task.CompletedTask;
        }
    }
}