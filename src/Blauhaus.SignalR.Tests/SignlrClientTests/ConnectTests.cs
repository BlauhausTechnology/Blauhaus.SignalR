using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.TestHelpers.Extensions;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Abstractions.Sync;
using Blauhaus.SignalR.Client;
using Blauhaus.SignalR.Client.Clients;
using Blauhaus.SignalR.Tests._Base;
using Blauhaus.SignalR.Tests.TestObjects;
using Moq;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.SignlRClientTests
{
    public class ConnectTests : BaseSignalRClientTest<SignalRClient<MyDto>>
    {
        
        private static List<MyDto> _publishedDtos = new();
        private IDictionary<string, string> _headers;
        
        private readonly Func<MyDto, Task> _handler = dto =>
        {
            _publishedDtos.Add(dto);
            return Task.CompletedTask;
        };

        private Guid _id;

        public override void Setup()
        {
            base.Setup();

            _id = Guid.NewGuid();
            _publishedDtos = new List<MyDto>();
            _headers = new Dictionary<string, string>{["Key"] = "Value"};
            MockAnalyticsService.With(x => x.AnalyticsOperationHeaders, _headers);

            MockSignalRConnectionProxy.AllowMockConnect();
        }

        [Test]
        public void SHOULD_Subscribe_to_connection_only_once()
        {
            //Act
            Sut.Connect();
            Sut.Connect();
            Sut.Connect();
            
            //Assert 
            MockSignalRConnectionProxy.Mock.Verify(x => x.Subscribe("PublishMyDtoAsync", It.IsAny<Func<MyDto, Task>>()), Times.Once);
        }
        
        [Test]
        public async Task WHEN_connection_publishes_later_response_SHOULD_update_subscribers_and_dto_cache()
        {
            //Arrange
            var dto1 = new MyDto(_id);
            var dto2 = new MyDto(_id);
            
            //Act
            Sut.Connect(); 
            await Sut.SubscribeAsync(_handler); 
            await MockSignalRConnectionProxy.MockPublishDtoAsync(dto1);
            await MockSignalRConnectionProxy.MockPublishDtoAsync(dto2);

            //Assert 
            Assert.That(_publishedDtos.Count, Is.EqualTo(2));
            Assert.That(_publishedDtos[0], Is.EqualTo(dto1));
            Assert.That(_publishedDtos[1], Is.EqualTo(dto2));
            MockMyDtoCache.VerifySaveAsync(dto1);
            MockMyDtoCache.VerifySaveAsync(dto2);
        }
         
        
        [Test]
        public async Task WHEN_connection_publishes_later_response_AND_subscription_is_disposed_SHOULD_not_update_subscribers()
        {
            //Arrange
            var dto1 = new MyDto(_id);
            var dto2 = new MyDto(_id);
            
            //Act
            var disposable = await Sut.SubscribeAsync(_handler); 
            disposable.Dispose();
            await MockSignalRConnectionProxy.MockPublishDtoAsync(dto1);
            await MockSignalRConnectionProxy.MockPublishDtoAsync(dto2);

            //Assert 
            Assert.That(_publishedDtos.Count, Is.EqualTo(0)); 
        }
         
    }
}