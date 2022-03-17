using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
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
using Microsoft.Extensions.Logging;

namespace Blauhaus.SignalR.Server.Hubs
{
    public abstract class BaseSignalRHub<THub> : Hub where THub : BaseSignalRHub<THub>
    {
        protected readonly IAnalyticsLogger<THub> Logger;
        protected readonly IServiceLocator ServiceLocator;
        protected  readonly IConnectedUserFactory UserFactory;

        protected BaseSignalRHub(
            IAnalyticsLogger<THub> logger,
            IServiceLocator serviceLocator, 
            IConnectedUserFactory userFactory)
        {
            Logger = logger;
            ServiceLocator = serviceLocator;
            UserFactory = userFactory;
        }


        protected async Task<Response> HandleVoidCommandAsync<TCommand, TId>(
            TCommand command, 
            Dictionary<string, object> headers, 
            Expression<Func<TCommand, IConnectedUser, TId>> idResolver,
            Func<TId, IVoidAuthenticatedCommandHandler<TCommand, IConnectedUser>> handlerResolver, 
            string? messageTemplate = null, params object[] args) 
                where TCommand : notnull
        {
            Logger.SetValues(headers);
            if (messageTemplate == null)
            {
                messageTemplate = "Hub handled command {CommandType}";
                args = new object[] { typeof(TCommand).Name };
            }

            using var _ = Logger.BeginTimedScope(LogLevel.Trace, messageTemplate, args);
            
            try
            {
                var connectedUser = GetConnectedUser();
                var id = idResolver.Compile().Invoke(command, connectedUser);
                var handler = handlerResolver.Invoke(id);

                Logger.SetValue("UserId", connectedUser.UserId);
                Logger.SetValue("ConnectionId", connectedUser.CurrentConnectionId);

                return await handler.HandleAsync(command, connectedUser);
            }
            catch (ErrorException error)
            {
                return Logger.LogErrorResponse(error.Error);
            }
            catch (Exception e)
            {
                return Logger.LogErrorResponse(Error.Unexpected(e.Message), e);
            }
        }

        protected async Task<Response<TResponse>> HandleCommandAsync<TResponse, TCommand, TId>(
            TCommand command, 
            Dictionary<string, object> headers, 
            Expression<Func<TCommand, IConnectedUser, TId>> idResolver,
            Func<TId, IAuthenticatedCommandHandler<TResponse, TCommand, IConnectedUser>> handlerResolver, 
            string? messageTemplate = null, params object[] args) 
                where TCommand : notnull
        {
            Logger.SetValues(headers);
            if (messageTemplate == null)
            {
                messageTemplate = "Hub handled command {CommandType} for response {ResponseType}";
                args = new object[] { typeof(TCommand).Name, typeof(TResponse).Name };
            }

            using var _ = Logger.BeginTimedScope(LogLevel.Trace, messageTemplate, args);


            try
            {
                var connectedUser = GetConnectedUser();
                var id = idResolver.Compile().Invoke(command, connectedUser);
                var handler = handlerResolver.Invoke(id);

                return await handler.HandleAsync(command, connectedUser);
            }
            catch (ErrorException error)
            {
                return Logger.LogErrorResponse<TResponse>(error.Error);
            }
            catch (Exception e)
            {
                return Logger.LogErrorResponse<TResponse>(Error.Unexpected(e.Message), e);
            }
        } 
         
        protected IConnectedUser GetConnectedUser()
        {
            return UserFactory.ExtractFromHubContext(Context); 
        }
    }
}