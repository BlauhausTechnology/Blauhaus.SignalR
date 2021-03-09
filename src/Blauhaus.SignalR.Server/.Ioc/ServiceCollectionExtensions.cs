using Blauhaus.SignalR.Abstractions.Server;
using Blauhaus.SignalR.Server.Proxy;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.SignalR.Server._Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSignalRHub<THub>(this IServiceCollection services) where THub : Hub
        {
            services.AddScoped<IHubContextProxy, HubContextProxy<THub>>();
            return services;
        }
    }
}