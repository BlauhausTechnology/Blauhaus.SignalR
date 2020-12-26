using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.TestHelpers.Extensions;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Abstractions.Sync;
using Blauhaus.SignalR.Client;
using Blauhaus.SignalR.Tests._Base;
using Blauhaus.SignalR.Tests.TestObjects;
using Moq;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.SignlRClientTests
{
    public class ConnectAsyncTests : BaseSignalRClientTest<SignalRClient<MyDto>>
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

            MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Success(new MyDto(_id)));
        }

        [Test]
        public async Task SHOULD_invoke_connect_command_even_if_id_already_connected_but_should_only_subscribe_once_for_updates()
        {
            //Arrange
            var otherId = Guid.NewGuid();
            
            //Act
            await Sut.ConnectAsync(_id, _handler);
            await Sut.ConnectAsync(_id, _handler);
            await Sut.ConnectAsync(otherId, _handler);
            
            //Assert
            MockSignalRConnectionProxy.Mock.Verify(x => x.InvokeAsync<Response<MyDto>>("ConnectMyDtoAsync", _id, _headers), Times.Exactly(2));
            MockSignalRConnectionProxy.Mock.Verify(x => x.InvokeAsync<Response<MyDto>>("ConnectMyDtoAsync", otherId, _headers), Times.Once);
            MockSignalRConnectionProxy.Mock.Verify(x => x.Subscribe("ConnectMyDtoAsync", It.IsAny<Func<MyDto, Task>>()), Times.Once);
        }
        
        [Test]
        public async Task WHEN_subscription_returns_initial_Dto_SHOULD_update_subscribers_and_dto_cache()
        {
            //Arrange
            var dto1 = new MyDto();
            MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Success(dto1));
            var dtosForSecondSubscription = new List<MyDto>();
            
            //Act
            await Sut.ConnectAsync(_id, _handler); 
            await Sut.ConnectAsync(_id, publishedDto =>
            {
                dtosForSecondSubscription.Add(publishedDto);
                return Task.CompletedTask;
            }); 

            //Assert 
            Assert.That(_publishedDtos.Count, Is.EqualTo(2));           // 2 because it gets another one when the second subscriber is updated
            Assert.That(_publishedDtos[1], Is.EqualTo(dto1));
            Assert.That(dtosForSecondSubscription.Count, Is.EqualTo(1));
            Assert.That(dtosForSecondSubscription[0], Is.EqualTo(dto1));
            MockMyDtoCache.VerifySaveAsync(dto1, 2);
        }
        
        [Test]
        public async Task WHEN_connection_publishes_later_response_SHOULD_update_subscribers_and_dto_cache()
        {
            //Arrange
            var dto1 = new MyDto();
            var dto2 = new MyDto();
            
            //Act
            await Sut.ConnectAsync(_id, _handler); 
            await MockSignalRConnectionProxy.PublishMockConnectAsync(dto1);
            await MockSignalRConnectionProxy.PublishMockConnectAsync(dto2);

            //Assert 
            Assert.That(_publishedDtos.Count, Is.EqualTo(3));
            Assert.That(_publishedDtos[1], Is.EqualTo(dto1));
            Assert.That(_publishedDtos[2], Is.EqualTo(dto2));
            MockMyDtoCache.VerifySaveAsync(dto1);
            MockMyDtoCache.VerifySaveAsync(dto2);
        }
        
        [Test]
        public async Task WHEN_connection_publishes_later_response_AND_subscription_is_disposed_SHOULD_not_update_subscribers()
        {
            //Arrange
            var dto1 = new MyDto();
            var dto2 = new MyDto();
            
            //Act
            var disposable = await Sut.ConnectAsync(_id, _handler); 
            disposable.Value.Dispose();
            await MockSignalRConnectionProxy.PublishMockConnectAsync(dto1);
            await MockSignalRConnectionProxy.PublishMockConnectAsync(dto2);

            //Assert 
            Assert.That(_publishedDtos.Count, Is.EqualTo(1));
            Assert.That(_publishedDtos[0], Is.Not.EqualTo(dto1));
            Assert.That(_publishedDtos[0], Is.Not.EqualTo(dto2));
        }
        
        [Test]
        public async Task WHEN_subscription_is_disposed_SHOULD_disconnect_from_server()
        {
            //Arrange 
            
            //Act
            var disposable = await Sut.ConnectAsync(_id, _handler); 
            disposable.Value.Dispose(); 

            //Assert 
            MockSignalRConnectionProxy.Mock.Verify(x => x.InvokeAsync("DisconnectMyDtoAsync", _id, _headers));
        }
        
        [Test]
        public async Task WHEN_subscription_fails_SHOULD_fail()
        {
            //Arrange
            MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Failure<MyDto>(Errors.Errors.Cancelled));
            
            //Act
            var result = await Sut.ConnectAsync(_id, _handler); 

            //Assert 
            Assert.That(result.Error, Is.EqualTo(Errors.Errors.Cancelled));
        }
        
        [Test]
        public async Task WHEN_subscription_throws_ErrorException_SHOULD_return_error()
        {
            //Arrange 
            MockSignalRConnectionProxy.Where_InvokeAsync_throws<Response<MyDto>>(new ErrorException(Errors.Errors.Cancelled));
            
            //Act
            var result = await Sut.ConnectAsync(_id, _handler); 

            //Assert 
            result.VerifyResponseError(Errors.Errors.Cancelled, MockAnalyticsService);
        }
        
        [Test]
        public async Task WHEN_subscription_throws_Exception_SHOULD_fail()
        {
            //Arrange 
            var unhandledException = new Exception("oops");
            MockSignalRConnectionProxy.Where_InvokeAsync_throws<Response<MyDto>>(unhandledException);
            
            //Act
            var result = await Sut.ConnectAsync(_id, _handler); 

            //Assert 
            Assert.That(result.Error.Equals(SignalRErrors.InvocationFailure(unhandledException)));
            MockAnalyticsService.VerifyLogExceptionWithMessage("oops");
        }
         
    }
}