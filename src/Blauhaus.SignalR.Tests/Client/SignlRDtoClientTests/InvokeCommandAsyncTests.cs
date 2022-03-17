using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Client.Clients;
using Blauhaus.SignalR.Tests.Base;
using Blauhaus.SignalR.Tests.Client.SignlRDtoClientTests.Base;
using Blauhaus.SignalR.Tests.TestObjects;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.Client.SignlRDtoClientTests
{
    public class InvokeCommandAsyncTests : BaseSignalRDtoClientTest
    {
        private MyCommand _command = null!;
        private Dictionary<string, object> _headers = null!;
        
        public override void Setup()
        {
            base.Setup();

            _command = new MyCommand();
            _headers = new Dictionary<string, object>{["Key"] = "Value"};
            MockAnalyticsContext.Where_GetAllValues_returns(_headers);
            MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Success(new List<MyDto>()));
        }
        
        private Task<Response<MyDto>> ExecuteAsync()
        {
            return Sut.HandleCommandAsync(_command);
        }

        [Test]
        public async Task SHOULD_invoke_given_method_name_and_command_and_append_analyticsHeaders()
        {
            //Act
            await ExecuteAsync();

            //Assert
            MockSignalRConnectionProxy.Mock.Verify(x => x.InvokeAsync<Response<MyDto>>("HandleMyCommandAsync", _command, _headers));
        }
        
        [Test]
        public async Task IF_hub_invocation_succeeds_SHOULD_return_response()
        {
            //Arrange
            var dto = new MyDto();
            MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Success(dto));

            //Act
            var result = await ExecuteAsync();

            //Assert
            Assert.That(result.Value, Is.EqualTo(dto));
        }
        
        [Test]
        public async Task IF_hub_invocation_succeeds_SHOULD_update_cache()
        {
            //Arrange
            var dto = new MyDto();
            MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Success(dto));

            //Act
            await ExecuteAsync();

            //Assert
            MockDtoCache.Mock.Verify(x => x.HandleAsync(dto));
        }

        [Test]
        public async Task IF_hub_invocation_succeeds_SHOULD_notify_subscribers()
        {
            //Arrange
            var dto = new MyDto();
            MyDto? incomingDto = null;
            MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Success(dto));
            await Sut.SubscribeAsync(x =>
            {
                incomingDto = x;
                return Task.CompletedTask;
            });

            //Act
            await ExecuteAsync();

            //Assert
            Assert.That(incomingDto!=null);
            Assert.That(incomingDto!.Id, Is.EqualTo(dto.Id));
        }
        [Test]
        public async Task IF_hub_invocation_succeeds_SHOULD_not_notify_ex_subscribers()
        {
            //Arrange
            var dto = new MyDto();
            MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Success(dto));
            MyDto? incomingDto = null;
            var token = await Sut.SubscribeAsync(x =>
            {
                incomingDto = x;
                return Task.CompletedTask;
            });
            token.Dispose();

            //Act
            await ExecuteAsync();

            //Assert
            Assert.That(incomingDto, Is.Null);
        }

        [Test]
        public async Task IF_device_is_disconnected_from_internet_SHOULD_return_Error()
        {
            //Arrange
            MockConnectivityService.With(x => x.IsConnectedToInternet, false);

            //Act
            var result = await ExecuteAsync();

            //Assert
            Assert.That(result.Error == SignalRErrors.NoInternet);
            MockLogger.VerifyLog("SignalR hub could not be invoked because there is no internet connection");
        }
        
        [Test]
        public async Task IF_connection_throws_exception_SHOULD_return_Error()
        {
            //Arrange
            var e = new Exception("Something bad happened");
            MockSignalRConnectionProxy.Where_InvokeAsync_throws<Response<MyDto>>(e);

            //Act
            var result = await ExecuteAsync();

            //Assert
            MockLogger.VerifyLogErrorResponse(SignalRErrors.InvocationFailure(e), result, e);
        } 
        
        [Test]
        public async Task IF_connection_throws_error_exception_SHOULD_return_Error()
        {
            //Arrange
            var e = new ErrorException(Error.Cancelled);
            MockSignalRConnectionProxy.Where_InvokeAsync_throws<Response<MyDto>>(e);

            //Act
            var result = await ExecuteAsync();

            //Assert
            MockLogger.VerifyLogErrorResponse(Error.Cancelled, result);
        } 
    }
}