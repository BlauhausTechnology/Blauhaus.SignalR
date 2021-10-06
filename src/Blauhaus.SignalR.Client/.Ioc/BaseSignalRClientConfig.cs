namespace Blauhaus.SignalR.Client.Ioc
{
    public abstract class BaseSignalRClientConfig : ISignalRClientConfig
    {
        protected BaseSignalRClientConfig(
            string hubUrl, 
            bool isAutoReconnectEnabled = false, 
            bool isTraceLoggingRequired = false, 
            bool bypassAndroidSslErrors = false)
        {
            HubUrl = hubUrl;
            IsAutoReconnectEnabled = isAutoReconnectEnabled;
            IsTraceLoggingRequired = isTraceLoggingRequired;
            BypassAndroidSSLErrors = bypassAndroidSslErrors;
        }

        public string HubUrl { get; }
        public bool IsAutoReconnectEnabled { get; protected set; }
        public bool IsTraceLoggingRequired { get; protected set; }
        public bool BypassAndroidSSLErrors { get; protected set; }
    }
}