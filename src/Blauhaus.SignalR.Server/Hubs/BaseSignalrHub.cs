using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Domain.Abstractions.CommandHandlers;
using Blauhaus.Errors;
using Blauhaus.Ioc.Abstractions;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Auth;
using Blauhaus.SignalR.Server.Auth;
using Microsoft.AspNetCore.SignalR;

namespace Blauhaus.SignalR.Server.Hubs
{
    public abstract class BaseSignalRHub : Hub
    {
        protected readonly IServiceLocator ServiceLocator;
        protected readonly IAnalyticsService AnalyticsService;
        private readonly IAuthenticatedUserFactory _authenticatedUserFactory; 

        protected BaseSignalRHub(
            IServiceLocator serviceLocator, 
            IAnalyticsService analyticsService,
            IAuthenticatedUserFactory authenticatedUserFactory)
        {
            ServiceLocator = serviceLocator;
            AnalyticsService = analyticsService;
            _authenticatedUserFactory = authenticatedUserFactory; 
        }

        protected async Task<Response<TResponse>> HandleCommandAsync<TResponse, TCommand>(
            TCommand command, 
            IDictionary<string, string> headers, 
            Expression<Func<TCommand, IConnectedUser, Guid>> idResolver,
            Func<Guid, IAuthenticatedCommandHandler<TResponse, TCommand, IConnectedUser>> handlerResolver)  
        {
            using (var _ = AnalyticsService.StartRequestOperation(this, typeof(TCommand).Name, headers))
            {
                try
                {
                    var connectedUser = GetConnectedUser();
                    var id = idResolver.Compile().Invoke(command, connectedUser);
                    var handler = handlerResolver.Invoke(id);

                    return await handler.HandleAsync(command, connectedUser);
                }
                catch (ErrorException error)
                {
                    return AnalyticsService.TraceErrorResponse<TResponse>(this, error.Error, command.ToObjectDictionary());
                }
                catch (Exception e)
                {
                    return AnalyticsService.LogExceptionResponse<TResponse>(this, e, Errors.Errors.Unexpected(e.Message), command.ToObjectDictionary());
                }
            }
        }



        protected IConnectedUser GetConnectedUser()
        {
            
            var deviceIdentifier = Context.GetHttpContext().Request.Query["device"];
            if (string.IsNullOrEmpty(deviceIdentifier))
            {
                throw new InvalidOperationException("No device identifier");
            }

            var getUserResult = _authenticatedUserFactory.ExtractFromClaimsPrincipal(Context.User ?? throw new InvalidOperationException("Invalid user in Context"));
            if (getUserResult.IsFailure)
            {
                throw new InvalidOperationException("No connected user");
            }

            var authenticatedUser = getUserResult.Value;
            return new ConnectedUser(authenticatedUser, deviceIdentifier, Context.ConnectionId);
        }
    }
}