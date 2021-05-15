using Blauhaus.SignalR.Abstractions.Client;

namespace Blauhaus.SignalR.Abstractions.Extensions
{
    public static class SignalRConnectionStateExtensions
    {
        public static bool IsConnecting(this SignalRConnectionState state)
        {
            return state is SignalRConnectionState.Connecting or SignalRConnectionState.Reconnecting;
        }

        public static bool IsConnected(this SignalRConnectionState state)
        {
            return state is SignalRConnectionState.Connected or SignalRConnectionState.Reconnected;
        }
        
        public static bool IsDisconnecting(this SignalRConnectionState state)
        {
            return state is SignalRConnectionState.Disconnecting;
        }

        public static bool IsDisconnected(this SignalRConnectionState state)
        {
            return state is SignalRConnectionState.Disconnected;
        }
    }
}