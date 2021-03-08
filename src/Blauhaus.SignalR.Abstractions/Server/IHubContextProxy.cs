using System.Threading.Tasks;
using Blauhaus.Responses;

namespace Blauhaus.SignalR.Abstractions.Server
{
    public interface IHubContextProxy
    {
        Task<Response> PublishDtoAsync<TDto>(string connectiondId, TDto dto);
    }
}