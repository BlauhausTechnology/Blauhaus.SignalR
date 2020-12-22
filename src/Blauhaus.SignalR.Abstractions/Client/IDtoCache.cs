
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Common.Utils.Contracts;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface IDtoCache<in TDto>  
    {
        Task SaveAsync(TDto dto);
    }
}