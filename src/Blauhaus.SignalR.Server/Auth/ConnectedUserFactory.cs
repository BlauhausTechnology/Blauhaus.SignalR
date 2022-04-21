using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Auth;
using Microsoft.AspNetCore.SignalR;
using System;
using Blauhaus.Analytics.Abstractions;
using Microsoft.Extensions.Logging;

namespace Blauhaus.SignalR.Server.Auth
{
    public class ConnectedUserFactory : IConnectedUserFactory
    {
        private readonly IAnalyticsLogger<ConnectedUserFactory> _logger;
        private readonly IAuthenticatedUserFactory _authenticatedUserFactory;

        public ConnectedUserFactory(
            IAnalyticsLogger<ConnectedUserFactory> logger,
            IAuthenticatedUserFactory authenticatedUserFactory)
        {
            _logger = logger;
            _authenticatedUserFactory = authenticatedUserFactory;
        }

        public IConnectedUser ExtractFromHubContext(HubCallerContext context)
        {
            var deviceIdentifier = context.GetHttpContext().Request.Query["device"];
            if (string.IsNullOrEmpty(deviceIdentifier))
            {
                _logger.LogWarning("No device identifier was found in the Request Context");
                throw new InvalidOperationException("No device identifier");
            }

            var getUserResult = _authenticatedUserFactory.ExtractFromClaimsPrincipal(context.User ?? throw new InvalidOperationException("Invalid user in Context"));
            if (getUserResult.IsFailure)
            {
                throw new InvalidOperationException("No connected user");
            }

            var authenticatedUser = getUserResult.Value;
            
            return new ConnectedUser(authenticatedUser, deviceIdentifier, context.ConnectionId);
        }
    }
}