﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.SignalR.Abstractions.Client;

namespace Blauhaus.SignalR.Client.Connection.Registry
{
    public class SignalRDtoClientRegistry : ISignalRDtoClientRegistry
    {
        private readonly IEnumerable<ISignalRDtoClient> _clients;

        public SignalRDtoClientRegistry(IEnumerable<ISignalRDtoClient> clients)
        {
            _clients = clients;
        }
        
        public Task InitializeAllClientsAsync()
        {
            var tasks = new List<Task>();
            foreach (var initializationTask in _clients)
            {
                tasks.Add(initializationTask.InitializeAsync());
            }
            return Task.WhenAll(tasks);
        }
    }
}