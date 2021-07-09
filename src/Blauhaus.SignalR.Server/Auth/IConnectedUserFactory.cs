using Blauhaus.Auth.Abstractions.User;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Auth;
using Microsoft.AspNetCore.SignalR;

namespace Blauhaus.SignalR.Server.Auth
{
    public interface IConnectedUserFactory
    {
        IConnectedUser ExtractFromHubConext(HubCallerContext context);
    }
}