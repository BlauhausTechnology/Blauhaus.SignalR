﻿using System;
using Blauhaus.Common.Abstractions;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Client.Clients;
using Blauhaus.SignalR.Client.Connection;
using Blauhaus.SignalR.Client.Connection.Proxy;
using Blauhaus.SignalR.Client.Connection.Registry;
using Blauhaus.SignalR.Client.DtoCache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Blauhaus.SignalR.Client.Ioc
{
    public static class ServiceCollectionExtensions
    {
        //Client
        public static IServiceCollection AddSignalRDtoClient<TDto, TId>(this IServiceCollection services) 
            where TDto : class, IHasId<TId>
        {
            services.AddSingleton<ISignalRDtoClient<TDto>, SignalRDtoClient<TDto, TId>>();
            services.AddSingleton<ISignalRDtoClient>(sp => sp.GetRequiredService<ISignalRDtoClient<TDto>>());
            services.AddDtoSaver<TDto, InMemoryDtoCache<TDto, TId>>();
            return services;
        }

        public static IServiceCollection AddSignalRDtoClient<TDto, TDtoSaver, TId>(this IServiceCollection services, Func<IServiceProvider, TDtoSaver> dtoSaverResolver) 
            where TDtoSaver : class, IDtoSaver<TDto> 
            where TDto : class, IHasId<TId>
        {
            services.AddSingleton<ISignalRDtoClient<TDto>, SignalRDtoClient<TDto, TId>>();
            services.AddSingleton<ISignalRDtoClient>(sp => sp.GetRequiredService<ISignalRDtoClient<TDto>>());
            services.AddSingleton<IDtoSaver<TDto>>(dtoSaverResolver.Invoke);
            return services;
        }
        
        public static IServiceCollection AddSignalRDtoClient<TDto, TDtoSaver, TId>(this IServiceCollection services) 
            where TDtoSaver : class, IDtoSaver<TDto> 
            where TDto : class, IHasId<TId>
        {
            services.AddSingleton<ISignalRDtoClient<TDto>, SignalRDtoClient<TDto, TId>>();
            services.AddSingleton<ISignalRDtoClient>(sp => sp.GetRequiredService<ISignalRDtoClient<TDto>>());
            services.AddDtoSaver<TDto, TDtoSaver>();
            return services;
        }
        
        private static IServiceCollection AddDtoSaver<TDto, TDtoSaver>(this IServiceCollection services) 
            where TDtoSaver : class, IDtoSaver<TDto>  
        {
            services.AddSingleton<IDtoSaver<TDto>, TDtoSaver>();
            return services;
        }
         
        
        //Services

        public static IServiceCollection AddSignalR<TConfig>(this IServiceCollection services)
            where TConfig : class, ISignalRClientConfig
        {
            services.TryAddTransient<ISignalRClientConfig, TConfig>();
            services.TryAddSingleton<ISignalRConnectionProxy, SignalRConnectionProxy>();
            services.TryAddSingleton<ISignalRClient, SignalRClient>();
            services.TryAddSingleton<ISignalRDtoClientRegistry, SignalRDtoClientRegistry>();

            return services;
        }
        
        
        //Sync
        
        public static IServiceCollection AddSignalRSyncClient<TDto, TDtoCache>(this IServiceCollection services) 
            where TDtoCache : class, ISyncDtoCache<TDto>
            where TDto : class, IClientEntity
        {
            services.AddSingleton<ISignalRSyncDtoClient<TDto>, SignalRSyncDtoClient<TDto>>();
            services.AddSyncDtoCache<TDto, TDtoCache>();
            return services;
        }
        
         
        private static IServiceCollection AddSyncDtoCache<TDto, TDtoCache>(this IServiceCollection services) 
            where TDtoCache : class, ISyncDtoCache<TDto> where TDto : class, IClientEntity
        {
            services.AddSingleton<ISyncDtoCache<TDto>, TDtoCache>();
            
            return services;
        }

    }
}