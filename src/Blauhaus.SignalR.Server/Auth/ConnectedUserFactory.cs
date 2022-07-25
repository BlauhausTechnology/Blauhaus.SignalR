using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Auth;
using Microsoft.AspNetCore.SignalR;
using System;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Errors;
using Blauhaus.SignalR.Abstractions.Client;
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

        public Response<IConnectedUser> ExtractFromHubContext(HubCallerContext context)
        {
            var httpContext = context.GetHttpContext();
            try
            {
                var deviceIdentifier = httpContext.Request.Query["device"];
                if (string.IsNullOrEmpty(deviceIdentifier))
                {
                    _logger.LogWarning("No device identifier was found in the Request Context");
                    return Response.Failure<IConnectedUser>(Error.RequiredValue("Device Identifier"));
                }
            
                var getUserResult = _authenticatedUserFactory.ExtractFromClaimsPrincipal(context.User ?? throw new InvalidOperationException("Invalid user in Context"));
                if (getUserResult.IsFailure)
                {
                    return Response.Failure<IConnectedUser>(getUserResult.Error);
                }

                var ipAddress = httpContext.Connection.RemoteIpAddress.ToString();
            
                var authenticatedUser = getUserResult.Value;
            
                return Response.Success<IConnectedUser>(new ConnectedUser(authenticatedUser, deviceIdentifier, context.ConnectionId, ipAddress));
            }
            catch (Exception e)
            {
                return _logger.LogErrorResponse<IConnectedUser>(SignalRErrors.HubConnectionFailed, e);
            }
        }
    }
}