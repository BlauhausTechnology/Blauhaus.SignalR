﻿using System;
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

namespace Blauhaus.SignalR.Tests.SignalRSyncClientTests
{
    public class SyncAsyncTests : BaseSignalRClientTest<SignalRSyncClient<MyDto>>
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

            _mockSubscription = MockSignalRConnectionProxy.AllowMockSync();

            MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Success(new SyncResponse<MyDto>()));
        }

        [Test]
        public async Task IF_client_is_not_subscribed_SHOULD_subscribe_to_connection()
        {
            //Arrange
            MockSyncMyDtoCache.Where_LoadSyncRequestAsync_returns(new SyncRequest(222));
            
            //Act
            await Sut.SyncAsync(_handler);
            await Sut.SyncAsync(_handler);
            
            //Assert
            MockSignalRConnectionProxy.Mock.Verify(x => x.InvokeAsync<Response<SyncResponse<MyDto>>>("SyncMyDtoAsync", It.Is<SyncRequest>(y=> 
                y.ModifiedAfter == 222), _headers), Times.Once);
            MockSignalRConnectionProxy.Mock.Verify(x => x.Subscribe("SyncMyDtoAsync", It.IsAny<Func<SyncResponse<MyDto>, Task>>()), Times.Once);
        }
        
        [Test]
        public async Task WHEN_subscription_returns_initial_Dtos_SHOULD_update_subscribers_and_dto_cache()
        {
            //Arrange
            var dto1 = new MyDto();
            var dto2 = new MyDto();
            MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Success(new SyncResponse<MyDto>(new []{dto1, dto2})));
            
            //Act
            await Sut.SyncAsync(_handler); 

            //Assert 
            Assert.That(_publishedDtos.Count, Is.EqualTo(2));
            Assert.That(_publishedDtos[0], Is.EqualTo(dto1));
            Assert.That(_publishedDtos[1], Is.EqualTo(dto2));
            MockSyncMyDtoCache.VerifySaveDtosAsync(dto1, dto2);
        }
        
        [Test]
        public async Task WHEN_connection_publishes_later_response_SHOULD_update_subscribers_and_dto_cache()
        {
            //Arrange
            var dto1 = new MyDto();
            var dto2 = new MyDto();
            
            //Act
            await Sut.SyncAsync(_handler);
            await MockSignalRConnectionProxy.PublishMockSyncAsync(new SyncResponse<MyDto>(new []{dto1}));
            await MockSignalRConnectionProxy.PublishMockSyncAsync(new SyncResponse<MyDto>(new []{dto2}));

            //Assert 
            Assert.That(_publishedDtos.Count, Is.EqualTo(2));
            Assert.That(_publishedDtos[0], Is.EqualTo(dto1));
            Assert.That(_publishedDtos[1], Is.EqualTo(dto2));
            MockSyncMyDtoCache.VerifySaveDtosAsync(dto1, dto2);
        }
        
        [Test]
        public async Task WHEN_subscription_fails_SHOULD_fail()
        {
            //Arrange
            MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Failure<SyncResponse<MyDto>>(Errors.Errors.Cancelled));
            
            //Act
            var result = await Sut.SyncAsync(_handler); 

            //Assert 
            Assert.That(result.Error, Is.EqualTo(Errors.Errors.Cancelled));
        }
        
        [Test]
        public async Task WHEN_subscription_throws_ErrorException_SHOULD_return_error()
        {
            //Arrange 
            MockSignalRConnectionProxy.Where_InvokeAsync_throws<Response<SyncResponse<MyDto>>>(new ErrorException(Errors.Errors.Cancelled));
            
            //Act
            var result = await Sut.SyncAsync(_handler); 

            //Assert 
            result.VerifyResponseError(Errors.Errors.Cancelled, MockAnalyticsService);
        }
        
        [Test]
        public async Task WHEN_subscription_throws_Exception_SHOULD_fail()
        {
            //Arrange 
            var unhandledException = new Exception("oops");
            MockSignalRConnectionProxy.Where_InvokeAsync_throws<Response<SyncResponse<MyDto>>>(unhandledException);
            
            //Act
            var result = await Sut.SyncAsync(_handler); 

            //Assert 
            Assert.That(result.Error.Equals(SignalRErrors.InvocationFailure(unhandledException)));
            MockAnalyticsService.VerifyLogExceptionWithMessage("oops");
        }
        
        
        [Test]
        public async Task IF_command_invocation_succeeds_SHOULD_update_subscribers()
        {
            //Arrange
            var dto = new MyDto();
            var publishedDtos = new List<MyDto>();
            MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Success(dto));
            await Sut.SyncAsync(async dto1 => publishedDtos.Add(dto1));
            await Sut.SyncAsync(async dto2 => publishedDtos.Add(dto2));

            //Act
            await Sut.HandleCommandAsync(new MyCommand());

            //Assert
            Assert.That(publishedDtos.Count, Is.EqualTo(2));
            Assert.That(publishedDtos[0], Is.EqualTo(dto));
            Assert.That(publishedDtos[1], Is.EqualTo(dto));
        }
    }
}