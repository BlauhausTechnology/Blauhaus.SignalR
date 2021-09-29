using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.Domain.Abstractions.CommandHandlers;
using Blauhaus.Domain.Abstractions.Sync;
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
        private readonly IConnectedUserFactory _userFactory;

        protected BaseSignalRHub(
            IServiceLocator serviceLocator, 
            IAnalyticsService analyticsService,
            IConnectedUserFactory userFactory)
        {
            ServiceLocator = serviceLocator;
            AnalyticsService = analyticsService;
            _userFactory = userFactory;
        }


        protected async Task<Response> HandleVoidCommandAsync<TCommand>(
            TCommand command, 
            IDictionary<string, string> headers, 
            Expression<Func<TCommand, IConnectedUser, Guid>> idResolver,
            Func<Guid, IVoidAuthenticatedCommandHandler<TCommand, IConnectedUser>> handlerResolver) 
                where TCommand : notnull
        {
            using var _ = AnalyticsService.StartRequestOperation(this, typeof(TCommand).Name, headers);
            try
            {
                var connectedUser = GetConnectedUser();
                var id = idResolver.Compile().Invoke(command, connectedUser);
                var handler = handlerResolver.Invoke(id);

                return await handler.HandleAsync(command, connectedUser);
            }
            catch (ErrorException error)
            {
                return AnalyticsService.TraceErrorResponse(this, error.Error, command.ToObjectDictionary());
            }
            catch (Exception e)
            {
                return AnalyticsService.LogExceptionResponse(this, e, Error.Unexpected(e.Message), command.ToObjectDictionary());
            }
        }

        protected async Task<Response<TResponse>> HandleCommandAsync<TResponse, TCommand>(
            TCommand command, 
            IDictionary<string, string> headers, 
            Expression<Func<TCommand, IConnectedUser, Guid>> idResolver,
            Func<Guid, IAuthenticatedCommandHandler<TResponse, TCommand, IConnectedUser>> handlerResolver)
        {
            using var _ = AnalyticsService.StartRequestOperation(this, typeof(TCommand).Name, headers);
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
                return AnalyticsService.LogExceptionResponse<TResponse>(this, e, Error.Unexpected(e.Message), command.ToObjectDictionary());
            }
        }

        protected async Task<Response<TResponse>> HandleDtoSyncCommandAsync<TResponse, TCommand>(
            TCommand command, 
            IDictionary<string, string> headers, 
            Expression<Func<TCommand, IConnectedUser, Guid>> idResolver,
            Func<Guid, IAuthenticatedCommandHandler<TResponse, TCommand, IConnectedUser>> handlerResolver)
        {
            using var _ = AnalyticsService.StartRequestOperation(this, typeof(TCommand).Name, headers);
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
                return AnalyticsService.LogExceptionResponse<TResponse>(this, e, Error.Unexpected(e.Message), command.ToObjectDictionary());
            }
        }
         
        protected IConnectedUser GetConnectedUser()
        {
            return _userFactory.ExtractFromHubContext(Context); 
        }
    }
}