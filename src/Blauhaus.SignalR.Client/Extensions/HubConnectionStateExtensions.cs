using System;
using Blauhaus.SignalR.Abstractions.Client;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.SignalR.Client.Extensions
{
    internal static class HubConnectionStateExtensions
    {
        internal static SignalRConnectionState ToConnectionState(this HubConnectionState state, HubConnectionState previousState)
        {
            switch (state)
            {
                case HubConnectionState.Connecting:
                    return SignalRConnectionState.Connecting;
                case HubConnectionState.Connected:
                    return previousState == HubConnectionState.Reconnecting ? SignalRConnectionState.Reconnected : SignalRConnectionState.Connected;
                case HubConnectionState.Disconnected:
                    return SignalRConnectionState.Disconnected;
                case HubConnectionState.Reconnecting:
                    return SignalRConnectionState.Reconnecting;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}