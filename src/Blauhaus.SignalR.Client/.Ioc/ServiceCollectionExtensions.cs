using System;
using Blauhaus.Common.Abstractions;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Client.Clients;
using Blauhaus.SignalR.Client.Connection;
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
            services.AddDtoCache<TDto, InMemoryDtoCache<TDto, TId>, TId>();
            return services;
        }
        
        public static IServiceCollection AddSignalRDtoClient<TDto, TDtoCache, TId>(this IServiceCollection services) 
            where TDtoCache : class, IDtoCache<TDto, TId> 
            where TDto : class, IHasId<TId>
        {
            services.AddSingleton<ISignalRDtoClient<TDto>, SignalRDtoClient<TDto, TId>>();
            services.AddDtoCache<TDto, TDtoCache, TId>();
            return services;
        }
        
        private static IServiceCollection AddDtoCache<TDto, TDtoCache, TId>(this IServiceCollection services) 
            where TDtoCache : class, IDtoCache<TDto, TId> where TDto : class, IHasId<TId>
        {
            services.AddSingleton<IDtoCache<TDto, TId>, TDtoCache>();
            return services;
        }
        
        //Services

        public static IServiceCollection AddSignalR<TConfig>(this IServiceCollection services)
            where TConfig : class, ISignalRClientConfig
        {
            services.TryAddTransient<ISignalRClientConfig, TConfig>();
            services.TryAddSingleton<ISignalRConnectionProxy, SignalRConnectionProxy>();
            services.TryAddSingleton<ISignalRClient, SignalRClient>();

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