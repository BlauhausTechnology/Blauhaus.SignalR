using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Analytics.TestHelpers.MockBuilders;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.DeviceServices.TestHelpers.MockBuilders;
using Blauhaus.SignalR.Client;
using Blauhaus.SignalR.TestHelpers.Extensions;
using Blauhaus.SignalR.TestHelpers.MockBuilders.DtoCaches;
using Blauhaus.SignalR.Tests.MockBuilders;
using Blauhaus.SignalR.Tests.TestObjects;
using Blauhaus.TestHelpers.BaseTests;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests._Base
{
    public abstract class BaseSignalRClientTest<TSut> : BaseServiceTest<TSut> where TSut : class
    {
        [SetUp]
        public virtual void Setup()
        {
            base.Cleanup();

            AddService(MockSignalRConnectionProxy.Object);
            AddService(MockAnalyticsService.Object);
            AddService(MockConnectivityService.Object);
            
            AddService(MockMyDtoCache.Object);
            AddService(MockSyncMyDtoCache.Object);
        }

        protected SignalRConnectionProxyMockBuilder MockSignalRConnectionProxy => AddMock<SignalRConnectionProxyMockBuilder, ISignalRConnectionProxy>().Invoke();
        protected AnalyticsServiceMockBuilder MockAnalyticsService => AddMock<AnalyticsServiceMockBuilder, IAnalyticsService>().Invoke();
        protected ConnectivityServiceMockBuilder MockConnectivityService => AddMock<ConnectivityServiceMockBuilder, IConnectivityService>().Invoke();

        protected DtoCacheMockBuilder<MyDto> MockMyDtoCache => Mocks.AddMockDtoCache<MyDto>().Invoke();
        protected SyncDtoCacheMockBuilder<MyDto> MockSyncMyDtoCache => Mocks.AddMockSyncDtoCache<MyDto>().Invoke();
    }
}