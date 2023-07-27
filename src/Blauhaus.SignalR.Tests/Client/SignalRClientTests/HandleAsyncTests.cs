using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Tests.Client.SignalRClientTests.Base;
using Blauhaus.SignalR.Tests.TestObjects;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.Client.SignalRClientTests
{
    [TestFixture]
    public class HandleAsyncTests : BaseSignalRClientTest
    {
        public override void Setup()
        {
            base.Setup();

            _command = new MyCommand();
            _headers = new Dictionary<string, object> {["Key"] = "Value"};
            MockAnalyticsContext.Where_GetAllValues_returns(_headers);
        }

        private MyCommand _command = null!;
        private Dictionary<string, object> _headers = null!;

        public class NoReturnValue : HandleAsyncTests
        {
            private async Task<Response> ExecuteAsync()
            {
                return await Sut.HandleVoidCommandAsync(_command);
            }

            [Test]
            public async Task SHOULD_invoke_given_method_name_and_command_and_append_analyticsHeaders()
            {
                //Act
                await ExecuteAsync();

                //Assert
                MockSignalRDtoConnectionProxy.Mock.Verify(x => x.InvokeAsync<Response>("HandleMyCommandAsync", _command, _headers));
            }

            [Test]
            public async Task IF_hub_invocation_succeeds_SHOULD_return_it()
            {
                //Arrange
                MockSignalRDtoConnectionProxy.Where_InvokeAsync_returns(Response.Success());

                //Act
                var result = await ExecuteAsync();

                //Assert
                Assert.That(result.IsSuccess);
            }

            [Test]
            public async Task IF_hub_invocation_fails_SHOULD_return_it()
            {
                //Arrange
                MockSignalRDtoConnectionProxy.Where_InvokeAsync_returns(Response.Failure(AuthError.NotAuthenticated));

                //Act
                var result = await ExecuteAsync();

                //Assert
                Assert.That(result.Error == AuthError.NotAuthenticated);
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
                MockSignalRDtoConnectionProxy.Where_InvokeAsync_throws<Response>(e);

                //Act
                var result = await ExecuteAsync();

                //Assert
                MockLogger.VerifyLogErrorResponse(SignalRErrors.InvocationFailure(e), result, e);
            }
        }

        public class ReturnValue : HandleAsyncTests
        {

            private class MyResponse
            {
            }

            private readonly MyResponse _myResponse = new();

             private async Task<Response<MyResponse>> ExecuteAsync()
            {
                return await Sut.HandleCommandAsync<MyCommand, MyResponse>(_command);
            }

            [Test]
            public async Task SHOULD_invoke_given_method_name_and_command_and_append_analyticsHeaders()
            {
                //Act
                await ExecuteAsync();

                //Assert
                MockSignalRDtoConnectionProxy.Mock.Verify(x => x.InvokeAsync<Response<MyResponse>>("HandleMyCommandAsync", _command, _headers));
            }

            [Test]
            public async Task IF_hub_invocation_succeeds_SHOULD_return_it()
            {
                //Arrange
                MockSignalRDtoConnectionProxy.Where_InvokeAsync_returns(Response.Success(_myResponse));

                //Act
                var result = await ExecuteAsync();

                //Assert
                Assert.That(result.IsSuccess);
                Assert.That(result.Value, Is.EqualTo(_myResponse));
            }

            [Test]
            public async Task IF_hub_invocation_fails_SHOULD_return_it()
            {
                //Arrange
                MockSignalRDtoConnectionProxy.Where_InvokeAsync_returns(Response.Failure<MyResponse>(AuthError.NotAuthenticated));

                //Act
                var result = await ExecuteAsync();

                //Assert
                Assert.That(result.Error == AuthError.NotAuthenticated);
            }

            [Test]
            public async Task IF_device_is_disconnected_from_internet_SHOULD_return_Error()
            {
                //Arrange
                MockConnectivityService.With(x => x.IsConnectedToInternet, false);

                //Act
                var result = await ExecuteAsync();

                //Assert
                MockLogger.VerifyLog("SignalR hub could not be invoked because there is no internet connection", LogLevel.Warning);
                Assert.That(result.Error, Is.EqualTo(SignalRErrors.NoInternet));
            }

            [Test]
            public async Task IF_connection_throws_exception_SHOULD_return_Error()
            {
                //Arrange
                var e = new Exception("Something bad happened");
                MockSignalRDtoConnectionProxy.Where_InvokeAsync_throws<Response<MyResponse>>(e);

                //Act
                var result = await ExecuteAsync();

                //Assert
                MockLogger.VerifyLogErrorResponse(SignalRErrors.InvocationFailure(e), result, e);
            }
        }
    }
}