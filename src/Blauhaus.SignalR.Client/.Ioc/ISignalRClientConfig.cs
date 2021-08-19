namespace Blauhaus.SignalR.Client._.Ioc
{
    public interface ISignalRClientConfig
    {
        string HubUrl { get; }
        bool IsAutoReconnectEnabled { get; }
        bool IsTraceLoggingRequired { get; }
    }
}