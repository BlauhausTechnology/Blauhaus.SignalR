﻿using System;
using System.Threading.Tasks;
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
        public static IServiceCollection AddSignalRDtoClient<TDto, TId, TDtoHandler>(this IServiceCollection services) 
            where TDto : class, IHasId<TId>
            where TDtoHandler : class, IDtoHandler<TDto>
        {
            services.AddSingleton<ISignalRDtoClient<TDto>, SignalRDtoClient<TDto, TId>>();
            services.AddSingleton<ISignalRDtoClient>(sp => sp.GetRequiredService<ISignalRDtoClient<TDto>>());
            services.AddDtoHandler<TDto, TId, TDtoHandler>();
            return services;
        }
        public static IServiceCollection AddDtoHandler<TDto, TId, TDtoHandler>(this IServiceCollection services) 
            where TDto : class, IHasId<TId>
            where TDtoHandler : class, IDtoHandler<TDto>
        {
            services.AddSingleton<Func<TId, Task<IDtoHandler<TDto>>>>(sp => id => Task.FromResult<IDtoHandler<TDto>>(sp.GetRequiredService<TDtoHandler>()));
            return services;
        }

        public static IServiceCollection AddSignalRDtoClient<TDto, TId>(this IServiceCollection services, Func<IServiceProvider, TId, Task<IDtoHandler<TDto>>> resolver) 
            where TDto : class, IHasId<TId>
        {
            services.AddSingleton<ISignalRDtoClient<TDto>, SignalRDtoClient<TDto, TId>>();
            services.AddSingleton<ISignalRDtoClient>(sp => sp.GetRequiredService<ISignalRDtoClient<TDto>>());
            services.AddSingleton<Func<TId, Task<IDtoHandler<TDto>>>>(sp => id => resolver.Invoke(sp, id));
            return services;
        }
        public static IServiceCollection AddDtoHandler<TDto, TId, TDtoHandler>(this IServiceCollection services, Func<IServiceProvider, TId, Task<IDtoHandler<TDto>>> resolver) 
            where TDto : class, IHasId<TId>
            where TDtoHandler : class, IDtoHandler<TDto>
        {
            services.AddSingleton<Func<TId, Task<IDtoHandler<TDto>>>>(sp => id => resolver.Invoke(sp, id));
            return services;
        }

        public static IServiceCollection AddSignalRDtoClient<TDto, TId>(this IServiceCollection services) 
            where TDto : class, IHasId<TId>
        {
            services.AddSingleton<ISignalRDtoClient<TDto>, SignalRDtoClient<TDto, TId>>();
            services.AddSingleton<ISignalRDtoClient>(sp => sp.GetRequiredService<ISignalRDtoClient<TDto>>());
            services.AddSingleton<IDtoCache<TDto, TId>, InMemoryDtoCache<TDto, TId>>();
            services.AddSingleton<Func<TId, Task<IDtoHandler<TDto>>>>(sp => id => Task.FromResult<IDtoHandler<TDto>>(sp.GetRequiredService<IDtoCache<TDto, TId>>()));
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
          
    }
}