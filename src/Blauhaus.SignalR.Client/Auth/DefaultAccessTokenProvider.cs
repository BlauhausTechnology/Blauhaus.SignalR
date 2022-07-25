using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Auth.Abstractions.AccessToken;
using Blauhaus.Auth.Abstractions.User;
using Blauhaus.SignalR.Abstractions.Auth;
using Microsoft.Extensions.Logging;

namespace Blauhaus.SignalR.Client.Auth;

public class DefaultAccessTokenProvider : IAccessTokenProvider
{
    private readonly IAnalyticsLogger<DefaultAccessTokenProvider> _logger;
    private readonly IAuthenticatedAccessToken _authenticatedAccessToken;

    public DefaultAccessTokenProvider(
        IAnalyticsLogger<DefaultAccessTokenProvider> logger,
        IAuthenticatedAccessToken authenticatedAccessToken)
    {
        _logger = logger;
        _authenticatedAccessToken = authenticatedAccessToken;
    }
    
    public Task<string?> GetAccessTokenAsync()
    {
        if (string.IsNullOrEmpty(_authenticatedAccessToken.Token))
        {
            _logger.LogDebug("IAuthenticatedAccessToken access token is null");
            return Task.FromResult<string?>(null);
        }
        return Task.FromResult<string?>(_authenticatedAccessToken.Token);
    }
}