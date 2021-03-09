using System.Threading.Tasks;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface IDtoCache<in TDto>  
    {
        Task SaveAsync(TDto dto);
    }
}