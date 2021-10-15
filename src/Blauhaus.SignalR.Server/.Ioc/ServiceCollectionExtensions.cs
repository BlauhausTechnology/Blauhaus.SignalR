using Blauhaus.Auth.Abstractions.Services;
using Blauhaus.SignalR.Abstractions.Server;
using Blauhaus.SignalR.Server.Auth;
using Blauhaus.SignalR.Server.Proxy;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.SignalR.Server.Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSignalRHub<THub>(this IServiceCollection services) where THub : Hub
        {
            services.AddScoped<IHubContextProxy, HubContextProxy<THub>>();
            services.AddScoped<IConnectedUserFactory, ConnectedUserFactory>();
            return services;
        }
    }
}