using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Analytics.TestHelpers.MockBuilders;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.DeviceServices.TestHelpers.MockBuilders;
using Blauhaus.SignalR.Client.Connection.Proxy;
using Blauhaus.SignalR.TestHelpers.MockBuilders;
using Blauhaus.SignalR.Tests.TestObjects;
using Blauhaus.TestHelpers.BaseTests;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.Base
{
    public abstract class BaseSignalRClientTest<TSut> : BaseServiceTest<TSut> where TSut : class
    {
        [SetUp]
        public virtual void Setup()
        {
            base.Cleanup();

            AddService(MockSignalRDtoConnectionProxy.Object);
            AddService(MockConnectivityService.Object);
            AddService(MockAnalyticsContext.Object);
            AddService(MockLogger.Object);
        }

        protected SignalRDtoConnectionProxyMockBuilder<MyDto> MockSignalRDtoConnectionProxy => AddMock<SignalRDtoConnectionProxyMockBuilder<MyDto>, ISignalRConnectionProxy>().Invoke();
        protected ConnectivityServiceMockBuilder MockConnectivityService => AddMock<ConnectivityServiceMockBuilder, IConnectivityService>().Invoke();
        protected AnalyticsContextMockBuilder MockAnalyticsContext => AddMock<AnalyticsContextMockBuilder, IAnalyticsContext>().Invoke();
        protected AnalyticsLoggerMockBuilder<TSut> MockLogger => AddMock<AnalyticsLoggerMockBuilder<TSut>, IAnalyticsLogger<TSut>>().Invoke();
    }
}