using Blauhaus.Domain.TestHelpers.MockBuilders.Client.DtoCaches;
using Blauhaus.SignalR.Client.Clients;
using Blauhaus.SignalR.Tests.Base;
using Blauhaus.SignalR.Tests.TestObjects;
using System;

namespace Blauhaus.SignalR.Tests.Client.SignlRDtoClientTests.Base
{
    public class BaseSignalRDtoClientTest : BaseSignalRClientTest<SignalRDtoClient<MyDto, Guid>>
    {
        
        protected DtoCacheMockBuilder<MyDto, Guid> MockDtoCache = null!;

        public override void Setup()
        {
            base.Setup();
            
            MockDtoCache = new DtoCacheMockBuilder<MyDto, Guid>();
            AddService(MockDtoCache.Object);
        }
    }
}