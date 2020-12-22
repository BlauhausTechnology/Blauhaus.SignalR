using Blauhaus.SignalR.Abstractions.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.SignalR.Client._Ioc
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddSignalRClient<TDto, TDtoSaver>(this IServiceCollection services) 
            where TDtoSaver : class, IDtoCache<TDto>
        {
            services.AddSingleton<ISignalRClient<TDto>, SignalRClient<TDto>>();
            services.AddDtoHandler<TDto, TDtoSaver>();
            
            return services;
        }
        
        public static IServiceCollection AddDtoHandler<TDto, TDtoSaver>(this IServiceCollection services) 
            where TDtoSaver : class, IDtoCache<TDto>
        {
            services.AddSingleton<IDtoCache<TDto>, TDtoSaver>();
            
            return services;
        }


        public static IServiceCollection AddSignalR<TConfig>(this IServiceCollection services)
            where TConfig : class, ISignalRClientConfig
        {
            services.AddTransient<ISignalRClientConfig, TConfig>();
            services.AddSingleton<ISignalRConnectionProxy, SignalRConnectionProxy>();
            services.AddSingleton<ISignalRCommandHandler, SignalRCommandHandler>();

            return services;
        }
    }
}