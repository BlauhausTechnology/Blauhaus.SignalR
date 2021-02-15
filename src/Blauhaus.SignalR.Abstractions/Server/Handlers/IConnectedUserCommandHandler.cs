using Blauhaus.Domain.Abstractions.CommandHandlers;
using Blauhaus.SignalR.Abstractions.Auth;

namespace Blauhaus.SignalR.Abstractions.Server.Handlers
{
    public interface IConnectedUserCommandHandler<in TCommand> : IVoidAuthenticatedCommandHandler<TCommand, IConnectedUser>
        where TCommand : notnull
    {
    }

    public interface IConnectedUserCommandHandler<TResponse, in TCommand> : IAuthenticatedCommandHandler<TResponse, TCommand, IConnectedUser>
        where TCommand : notnull
    {
    }
}