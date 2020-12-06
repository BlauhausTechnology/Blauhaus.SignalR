using Blauhaus.SignalR.Abstractions.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.SignalR.Client._Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSignalRClient<TConfig>(this IServiceCollection services)
            where TConfig : class, ISignalRClientConfig
        {
            services.AddTransient<ISignalRClientConfig, TConfig>();
            services.AddSingleton<ISignalRConnectionProxy, SignalRConnectionProxy>();
            services.AddSingleton<ISignalRCommandHandler, SignalRCommandHandler>();

            return services;
        }
    }
}