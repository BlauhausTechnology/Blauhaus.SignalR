using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Responses;
using Blauhaus.SignalR.Client;
using Blauhaus.SignalR.Tests._Base;
using Blauhaus.SignalR.Tests.TestObjects;
using Moq;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.SignlrClientTests
{
    public class SubscribeAsyncTests : BaseSignalRClientTest<SignalRClient<MyDto, MySubscribeCommand>>
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

            MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Success(new List<MyDto>()));
        }

        [Test]
        public async Task IF_client_is_not_subscribed_SHOULD_subscribe_to_connection()
        {
            //Arrange
            var subscription = new MySubscribeCommand();
            
            //Act
            await Sut.SubscribeAsync(subscription, _handler);
            await Sut.SubscribeAsync(subscription, _handler);
            
            //Assert
            MockSignalRConnectionProxy.Mock.Verify(x => x.InvokeAsync<Response<List<MyDto>>>("SubscribeToMyDto", subscription, _headers), Times.Once);
            MockSignalRConnectionProxy.Mock.Verify(x => x.Subscribe("PublishMyDto", It.IsAny<Func<MyDto, Task>>()), Times.Once);
        }
        
        [Test]
        public async Task WHEN_subscription_returns_initial_Dtos_SHOULD_update_subscribers_and_dto_cache()
        {
            //Arrange
            var dto1 = new MyDto();
            var dto2 = new MyDto();
            MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Success(new List<MyDto> {dto1, dto2}));
            
            //Act
            await Sut.SubscribeAsync(new MySubscribeCommand(), _handler); 

            //Assert 
            Assert.That(_publishedDtos.Count, Is.EqualTo(2));
            Assert.That(_publishedDtos[0], Is.EqualTo(dto1));
            Assert.That(_publishedDtos[1], Is.EqualTo(dto2));
            MockMyDtoCache.Mock.Verify(x => x.SaveAsync(dto1));
            MockMyDtoCache.Mock.Verify(x => x.SaveAsync(dto2));
        }
        
        [Test]
        public async Task WHEN_subscription_fails_SHOULD_fail()
        {
            //Arrange
            var dto1 = new MyDto();
            var dto2 = new MyDto();
            MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Failure<List<MyDto>>(Errors.Errors.Cancelled));
            
            //Act
            var result = await Sut.SubscribeAsync(new MySubscribeCommand(), _handler); 

            //Assert 
            Assert.That(result.Error, Is.EqualTo(Errors.Errors.Cancelled));
        }

        
        [Test]
        public async Task WHEN_connection_publishes_Dto_SHOULD_update_subscribers_and_dto_cache()
        {
            //Arrange
            var dto1 = new MyDto();
            var dto2 = new MyDto();
            
            //Act
            await Sut.SubscribeAsync(new MySubscribeCommand(), _handler);
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