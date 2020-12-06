using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.SignalR.Client
{
    public class ClientConnectionStateChangeEventArgs : EventArgs
    {
        public ClientConnectionStateChangeEventArgs(HubConnectionState state, Exception? exception)
        {
            State = state;
            Exception = exception;
        }

        public HubConnectionState State { get; }
        public Exception? Exception { get; }


    }
}