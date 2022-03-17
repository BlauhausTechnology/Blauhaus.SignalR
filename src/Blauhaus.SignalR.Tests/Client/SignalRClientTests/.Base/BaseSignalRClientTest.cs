using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Client.Connection;
using Blauhaus.SignalR.Client.Connection.Registry;
using Blauhaus.SignalR.Tests.Base;
using Blauhaus.TestHelpers.MockBuilders;

namespace Blauhaus.SignalR.Tests.Client.SignalRClientTests.Base
{
    public abstract class BaseSignalRClientTest : BaseSignalRClientTest<SignalRClient>
    {
        protected List<SignalRConnectionState> StateChanges = null!;
        protected Func<SignalRConnectionState, Task> Handler = null!;
        protected MockBuilder<ISignalRDtoClientRegistry> SignalRClientRegistry = null!;
        
        public override void Setup()
        {
            base.Setup();

            StateChanges = new List<SignalRConnectionState>();
            Handler = (connectionState) =>
            {
                StateChanges.Add(connectionState);
                return Task.CompletedTask;
            };
            SignalRClientRegistry = new MockBuilder<ISignalRDtoClientRegistry>();
            
            AddService(x => SignalRClientRegistry.Object);
        }

    }
}