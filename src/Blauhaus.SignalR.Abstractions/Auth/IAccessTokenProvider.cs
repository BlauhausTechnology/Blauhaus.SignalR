using System.Threading.Tasks;

namespace Blauhaus.SignalR.Abstractions.Auth;

public interface IAccessTokenProvider
{
    Task<string?> GetAccessTokenAsync();
}