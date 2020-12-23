﻿using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.SignalR.Abstractions.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Blauhaus.SignalR.Client._Ioc
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSignalRSyncClient<TDto, TDtoCache>(this IServiceCollection services) 
            where TDtoCache : class, ISyncDtoCache<TDto>
            where TDto : IClientEntity
        {
            services.AddSingleton<ISignalRSyncClient<TDto>, SignalRSyncClient<TDto>>();
            services.AddSyncDtoCache<TDto, TDtoCache>();
            return services;
        }
        
        public static IServiceCollection AddSignalRClient<TDto, TDtoCache>(this IServiceCollection services) 
            where TDtoCache : class, IDtoCache<TDto>
        {
            services.AddSingleton<ISignalRClient<TDto>, SignalRClient<TDto>>();
            services.AddDtoCache<TDto, TDtoCache>();
            return services;
        }
        
        public static IServiceCollection AddSyncDtoCache<TDto, TDtoCache>(this IServiceCollection services) 
            where TDtoCache : class, ISyncDtoCache<TDto> where TDto : IClientEntity
        {
            services.AddSingleton<ISyncDtoCache<TDto>, TDtoCache>();
            
            return services;
        }

        public static IServiceCollection AddDtoCache<TDto, TDtoCache>(this IServiceCollection services) 
            where TDtoCache : class, IDtoCache<TDto>
        {
            services.AddSingleton<IDtoCache<TDto>, TDtoCache>();
            
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