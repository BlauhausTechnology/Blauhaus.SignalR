using System;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Analytics.TestHelpers.MockBuilders;
using Blauhaus.DeviceServices.Abstractions.Connectivity;
using Blauhaus.DeviceServices.TestHelpers.MockBuilders;
using Blauhaus.Domain.Abstractions.DtoHandlers;
using Blauhaus.Domain.TestHelpers.Extensions;
using Blauhaus.Domain.TestHelpers.MockBuilders.Client.DtoCaches;
using Blauhaus.Domain.TestHelpers.MockBuilders.Client.DtoHandlers;
using Blauhaus.SignalR.Client.Connection.Proxy;
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
             
        }

        protected SignalRConnectionProxyMockBuilder MockSignalRConnectionProxy => AddMock<SignalRConnectionProxyMockBuilder, ISignalRConnectionProxy>().Invoke();
        protected AnalyticsServiceMockBuilder MockAnalyticsService => AddMock<AnalyticsServiceMockBuilder, IAnalyticsService>().Invoke();
        protected ConnectivityServiceMockBuilder MockConnectivityService => AddMock<ConnectivityServiceMockBuilder, IConnectivityService>().Invoke();
         
    }
}