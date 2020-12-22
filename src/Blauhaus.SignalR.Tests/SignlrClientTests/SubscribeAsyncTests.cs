using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.SignalR.Abstractions.Subscriptions;
using Blauhaus.SignalR.Client;
using Blauhaus.SignalR.Tests._Base;
using Blauhaus.SignalR.Tests.TestObjects;
using Moq;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.SignlrClientTests
{
    public class SubscribeAsyncTests : BaseSignalRClientTest<SignalRClient<MyDto>>
    {
        
        private static List<MyDto> _publishedDtos = new();
        private IDictionary<string, string> _headers;
        
        private readonly Func<MyDto, Task> _handler = dto =>
        {
            _publishedDtos.Add(dto);
            return Task.CompletedTask;
        };

        private Mock<IDisposable> _mockSubscription;

        public override void Setup()
        {
            base.Setup();

            _publishedDtos = new List<MyDto>();
            _headers = new Dictionary<string, string>{["Key"] = "Value"};
            MockAnalyticsService.With(x => x.AnalyticsOperationHeaders, _headers);

            _mockSubscription = MockSignalRConnectionProxy.AllowMockSubscriptions();
            
        }

        [Test]
        public async Task IF_client_is_not_subscribed_SHOULD_subscribe_to_connection()
        {
            //Arrange
            var subscription = new DtoSubscription();
            
            //Act
            await Sut.SubscribeAsync(_handler, subscription);
            await Sut.SubscribeAsync(_handler, subscription);
            
            //Assert
            MockSignalRConnectionProxy.Mock.Verify(x => x.InvokeAsync("SubscribeToMyDto", subscription, _headers), Times.Once);
            MockSignalRConnectionProxy.Mock.Verify(x => x.Subscribe<MyDto>("PublishMyDto", It.IsAny<Func<MyDto, Task>>()), Times.Once);
        }
        
        [Test]
        public async Task WHEN_connection_publishes_Dto_SHOULD_update_subscribers_and_dto_cache()
        {
            //Arrange
            var dto1 = new MyDto();
            var dto2 = new MyDto();
            
            //Act
            await Sut.SubscribeAsync(_handler);
            await MockSignalRConnectionProxy.PublishMockSubscriptionAsync(dto1);
            await MockSignalRConnectionProxy.PublishMockSubscriptionAsync(dto2);

            //Assert 
            Assert.That(_publishedDtos.Count, Is.EqualTo(2));
            Assert.That(_publishedDtos[0], Is.EqualTo(dto1));
            Assert.That(_publishedDtos[1], Is.EqualTo(dto2));
            MockMyDtoCache.Mock.Verify(x => x.SaveAsync(dto1));
            MockMyDtoCache.Mock.Verify(x => x.SaveAsync(dto2));
        }
    }
}