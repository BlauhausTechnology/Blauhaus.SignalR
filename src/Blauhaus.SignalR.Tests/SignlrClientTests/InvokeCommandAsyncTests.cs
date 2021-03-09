using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Client;
using Blauhaus.SignalR.Client.Clients;
using Blauhaus.SignalR.Tests._Base;
using Blauhaus.SignalR.Tests.TestObjects;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.SignlrClientTests
{
    public class InvokeCommandAsyncTests : BaseSignalRClientTest<SignalRClient<MyDto>>
    {
        private MyCommand _command;
        private IDictionary<string, string> _headers;
        
        public override void Setup()
        {
            base.Setup();

            _command = new MyCommand();
            _headers = new Dictionary<string, string>{["Key"] = "Value"};
            MockAnalyticsService.With(x => x.AnalyticsOperationHeaders, _headers);
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
            public async Task IF_hub_invocation_succeeds_SHOULD_update_dto_cache_with_dto()
            {
                //Arrange
                var dto = new MyDto();
                MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Success(dto));

                //Act
                await ExecuteAsync();

                //Assert
                MockMyDtoCache.Mock.Verify(x => x.SaveAsync(dto));
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
            
            [Test]
            public async Task IF_connection_throws_error_exception_SHOULD_return_Error()
            {
                //Arrange
                var e = new ErrorException(Errors.Errors.Cancelled);
                MockSignalRConnectionProxy.Where_InvokeAsync_throws<Response<MyDto>>(e);

                //Act
                var result = await ExecuteAsync();

                //Assert
                Assert.That(result.Error.Equals(Errors.Errors.Cancelled));
                MockAnalyticsService.VerifyTrace(Errors.Errors.Cancelled.ToString(), LogSeverity.Error);
            } 
    }
}