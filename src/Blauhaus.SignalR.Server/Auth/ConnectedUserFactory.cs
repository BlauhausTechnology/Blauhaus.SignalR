﻿using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Auth;
using Microsoft.AspNetCore.SignalR;
using System;

namespace Blauhaus.SignalR.Server.Auth
{
    public class ConnectedUserFactory : IConnectedUserFactory
    {
        private readonly IAuthenticatedUserFactory _authenticatedUserFactory;

        public ConnectedUserFactory(
            IAuthenticatedUserFactory authenticatedUserFactory)
        {
            _authenticatedUserFactory = authenticatedUserFactory;
        }

        public IConnectedUser ExtractFromHubContext(HubCallerContext context)
        {
            var deviceIdentifier = context.GetHttpContext().Request.Query["device"];
            if (string.IsNullOrEmpty(deviceIdentifier))
            {
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