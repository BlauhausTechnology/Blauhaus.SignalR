using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Client.Connection;
using Blauhaus.SignalR.Tests.Base;

namespace Blauhaus.SignalR.Tests.Client.SignalRConnectionTests.Base
{
    public abstract class BaseSignalRConnectionTest : BaseSignalRClientTest<SignalRConnection>
    {
        protected List<SignalRConnectionState> StateChanges;
        protected Func<SignalRConnectionState, Task> Handler;

        public override void Setup()
        {
            base.Setup();

            StateChanges = new List<SignalRConnectionState>();
            Handler = (connectionState) =>
            {
                StateChanges.Add(connectionState);
                return Task.CompletedTask;
            };
        }

    }
}