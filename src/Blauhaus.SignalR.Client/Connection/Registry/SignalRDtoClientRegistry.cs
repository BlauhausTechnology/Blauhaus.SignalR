using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blauhaus.SignalR.Client.Connection.Registry
{
    public class SignalRDtoClientRegistry : ISignalRDtoClientRegistry
    {
        private readonly List<Func<Task>> _initializationTasks = new List<Func<Task>>();

        internal void AddDtoClient(Func<Task> initializationFunc)
        {
            _initializationTasks.Add(initializationFunc);
        }
        
        
        public Task InitializeAllClientsAsync()
        {
            var tasks = new List<Task>();
            foreach (var initializationTask in _initializationTasks)
            {
                tasks.Add(initializationTask.Invoke());
            }
            return Task.WhenAll(tasks);
        }
    }
}