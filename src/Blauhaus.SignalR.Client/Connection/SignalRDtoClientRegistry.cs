using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.SignalR.Abstractions.Client;

namespace Blauhaus.SignalR.Client.Connection
{
    public class SignalRDtoClientRegistry : ISignalRDtoClientRegistry
    {
        private readonly List<Task> _initializationTasks = new List<Task>();

        internal void AddDtoClient(Task initializationFunc)
        {
            _initializationTasks.Add(initializationFunc);
        }
        
        
        public Task InitializeAllClientsAsync()
        {
            return Task.WhenAll(_initializationTasks);
        }
    }
}