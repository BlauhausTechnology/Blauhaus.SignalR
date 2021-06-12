using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.SignalR.Client.Clients;
using Blauhaus.SignalR.Tests.Base;
using Blauhaus.SignalR.Tests.TestObjects;
using Moq;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.Client.SignlRDtoClientTests
{
    public class InitializeAsyncTests : BaseSignalRClientTest<SignalRDtoClient<MyDto, Guid>>
    {
        
        private IDictionary<string, string> _headers = null!;
         
        public override void Setup()
        {
            base.Setup();

            _headers = new Dictionary<string, string>{["Key"] = "Value"};
            MockAnalyticsService.With(x => x.AnalyticsOperationHeaders, _headers);
             
        }

        [Test]
        public async Task SHOULD_Subscribe_to_connection_only_once()
        {
            //Act
            await Sut.InitializeAsync();
            await Sut.InitializeAsync();
            await Sut.InitializeAsync();
            
            //Assert 
            MockSignalRConnectionProxy.Mock.Verify(x => x.Subscribe("PublishMyDtoAsync", It.IsAny<Func<MyDto, Task>>()), Times.Once);
        }
         
    }
}