using System;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Analytics.TestHelpers.MockBuilders;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.DeviceServices.TestHelpers.MockBuilders;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Client.Connection;
using Blauhaus.SignalR.Client.Connection.Proxy;
using Blauhaus.SignalR.TestHelpers.Extensions;
using Blauhaus.SignalR.TestHelpers.MockBuilders.DtoCaches;
using Blauhaus.SignalR.TestHelpers.MockBuilders.DtoSavers;
using Blauhaus.SignalR.Tests.MockBuilders;
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

            AddService(MockSignalRConnectionProxy.Object);
            AddService(MockAnalyticsService.Object);
            AddService(MockConnectivityService.Object);
            
            AddService<Func<Guid, Task<IDtoSaver<MyDto>>>>(x => id => Task.FromResult(MockMyDtoCache.Object));
        }

        protected SignalRConnectionProxyMockBuilder MockSignalRConnectionProxy => AddMock<SignalRConnectionProxyMockBuilder, ISignalRConnectionProxy>().Invoke();
        protected AnalyticsServiceMockBuilder MockAnalyticsService => AddMock<AnalyticsServiceMockBuilder, IAnalyticsService>().Invoke();
        protected ConnectivityServiceMockBuilder MockConnectivityService => AddMock<ConnectivityServiceMockBuilder, IConnectivityService>().Invoke();

        protected DtoSaverMockBuilder<MyDto, Guid> MockMyDtoCache => Mocks.AddMockDtoSaver<MyDto, Guid>().Invoke();
    }
}