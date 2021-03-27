namespace Blauhaus.SignalR.Abstractions.Auth
{
    public static class ConnectedUserEvents
    {
        public static string UserConnected = nameof(UserConnected);
        public static string UserDisconnected = nameof(UserDisconnected);
    }
}