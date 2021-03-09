using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Auth.Abstractions.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Tests.Client.SignalRConnectionTests.Base;
using Blauhaus.SignalR.Tests.TestObjects;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.Client.SignalRConnectionTests
{
    [TestFixture]
    public class InvokeAsyncTests : BaseSignalRConnectionTest
    {
        private MyCommand _command;
        private IDictionary<string, string> _headers;

        public override void Setup()
        {
            base.Setup();

            _command = new MyCommand();
            _headers = new Dictionary<string, string>{["Key"] = "Value"};
            MockAnalyticsService.With(x => x.AnalyticsOperationHeaders, _headers);
        }

        public class NoDtoExpected : InvokeAsyncTests
        {

            private async Task<Response> ExecuteAsync()
            {
                return await Sut.HandleAsync(_command);
            }

            [Test]
            public async Task SHOULD_invoke_given_method_name_and_command_and_append_analyticsHeaders()
            {
                //Act
                await ExecuteAsync();

                //Assert
                MockSignalRConnectionProxy.Mock.Verify(x => x.InvokeAsync<Response>("HandleMyCommandAsync", _command, _headers));
            }

            [Test]
            public async Task IF_hub_invocation_succeeds_SHOULD_return_it()
            {
                //Arrange
                MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Success());

                //Act
                var result = await ExecuteAsync();

                //Assert
                Assert.That(result.IsSuccess);
            }

            [Test]
            public async Task IF_hub_invocation_fails_SHOULD_return_it()
            {
                //Arrange
                MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Failure(AuthErrors.NotAuthenticated));

                //Act
                var result = await ExecuteAsync();

                //Assert
                Assert.That(result.Error == AuthErrors.NotAuthenticated);
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
                MockAnalyticsService.VerifyTrace("SignalR hub could not be invoked because there is no internet connection", LogSeverity.Warning);
            }
            
            [Test]
            public async Task IF_connection_throws_exception_SHOULD_return_Error()
            {
                //Arrange
                var e = new Exception("Something bad happened");
                MockSignalRConnectionProxy.Where_InvokeAsync_throws<Response>(e);

                //Act
                var result = await ExecuteAsync();

                //Assert
                Assert.That(result.Error == SignalRErrors.InvocationFailure(e));
                MockAnalyticsService.VerifyLogExceptionWithMessage("Something bad happened");
            }
        }

        public class DtoExpected : InvokeAsyncTests
        {
            private Task<Response<MyDto>> ExecuteAsync()
            {
                return Sut.HandleAsync<MyDto, MyCommand>(_command);
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
            public async Task IF_hub_invocation_succeeds_SHOULD_return_it()
            {
                //Arrange
                MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Failure<MyDto>(AuthErrors.NotAuthenticated));

                //Act
                var result = await ExecuteAsync();

                //Assert
                Assert.That(result.Error == AuthErrors.NotAuthenticated);
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
                MockAnalyticsService.VerifyTrace("SignalR hub could not be invoked because there is no internet connection", LogSeverity.Warning);
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
                Assert.That(result.Error.Equals(SignalRErrors.InvocationFailure(e)));
                MockAnalyticsService.VerifyLogExceptionWithMessage("Something bad happened");
            }
        }

    }
}