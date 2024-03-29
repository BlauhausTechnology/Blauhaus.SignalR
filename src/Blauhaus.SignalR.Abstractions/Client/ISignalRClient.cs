﻿using System.Threading.Tasks;
using Blauhaus.Common.Abstractions;
using Blauhaus.Responses;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRClient : IAsyncPublisher<SignalRConnectionState>, IAsyncInitializable
    {
        Task InitializeAllClientsAsync();
        Task<Response> HandleVoidCommandAsync<TCommand>(TCommand command) where TCommand : notnull;
        Task<Response<TResponse>> HandleCommandAsync<TCommand, TResponse>(TCommand command) where TCommand : notnull;
        Task DisconnectAsync();

    }
     
}