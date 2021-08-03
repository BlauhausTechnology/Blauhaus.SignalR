namespace Blauhaus.SignalR.Client.Ioc
{
    public interface ISignalRClientConfig
    {
        string HubUrl { get; }
        bool IsAutoReconnectEnabled { get; }
        bool IsTraceLoggingRequired { get; }
    }
}