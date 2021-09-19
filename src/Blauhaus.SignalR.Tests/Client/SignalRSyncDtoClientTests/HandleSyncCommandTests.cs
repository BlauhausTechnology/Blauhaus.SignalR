using Blauhaus.SignalR.Client.Clients;
using Blauhaus.SignalR.Tests.Base;
using Blauhaus.SignalR.Tests.TestObjects;
using System;
using System.Collections.Generic;
using Blauhaus.Domain.Abstractions.Sync;
using Blauhaus.Domain.Client.Sync.DtoBatches;
using Blauhaus.Responses;
using System.Threading.Tasks;
using NUnit.Framework;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Errors;
using Blauhaus.SignalR.Abstractions.Client;

namespace Blauhaus.SignalR.Tests.Client.SignalRSyncDtoClientTests
{
    public class HandleSyncCommandTests : BaseSignalRClientTest<SignalRSyncDtoClient<MyDto, Guid>>
    {
        private DtoSyncCommand _command = null!;
        private IDictionary<string, string> _headers = null!;
        private MyDto _dto = null!;

        public override void Setup()
        {
            base.Setup();

            _command = new DtoSyncCommand(100, 10);
            _headers = new Dictionary<string, string>{["Key"] = "Value"};
            MockAnalyticsService.With(x => x.AnalyticsOperationHeaders, _headers);
            
            _dto = new MyDto{ModifiedAtTicks = 10001};
            MockSignalRConnectionProxy.Where_InvokeAsync_returns(Response.Success(new DtoBatch<MyDto, Guid>(new List<MyDto>
            {
                _dto
            }, 2)));
             
        }
        
        private Task<Response<IDtoBatch<MyDto>>> ExecuteAsync()
        {
            return Sut.HandleAsync(_command);
        }

        [Test]
        public async Task SHOULD_invoke_given_method_name_and_command_and_append_analyticsHeaders()
        {
            //Act
            await ExecuteAsync();

            //Assert
            MockSignalRConnectionProxy.Mock.Verify(x => x.InvokeAsync<Response<DtoBatch<MyDto, Guid>>>("HandleDtoSyncCommandAsync", _command, _headers));
        }
        
        [Test]
        public async Task IF_hub_invocation_succeeds_SHOULD_return_DtoBatch_info()
        { 
            //Act
            var result = await ExecuteAsync();

            //Assert
            Assert.That(result.Value.BatchLastModifiedTicks, Is.EqualTo(_dto.ModifiedAtTicks));
            Assert.That(result.Value.CurrentDtoCount, Is.EqualTo(1));
            Assert.That(result.Value.RemainingDtoCount, Is.EqualTo(2));
        }

        [Test]
        public async Task IF_hub_invocation_succeeds_SHOULD_invoke_handlers()
        { 
            //Act
            await ExecuteAsync();

            //Assert
            MockMyDtoHandler.Mock.Verify(x => x.HandleAsync(_dto));
        }

        [Test]
        public async Task IF_hub_invocation_succeeds_SHOULD_notify_subscribers()
        {
            //Arrange
            MyDto? incomingDto = null; 
            await Sut.SubscribeAsync(x =>
            {
                incomingDto = x;
                return Task.CompletedTask;
            });

            //Act
            await ExecuteAsync();

            //Assert
            Assert.That(incomingDto!=null);
            Assert.That(incomingDto!.Id, Is.EqualTo(_dto.Id));
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
            MockSignalRConnectionProxy.Where_InvokeAsync_throws<Response<DtoBatch<MyDto, Guid>>>(e);

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
            MockSignalRConnectionProxy.Where_InvokeAsync_throws<Response<DtoBatch<MyDto, Guid>>>(e);

            //Act
            var result = await ExecuteAsync();

            //Assert
            Assert.That(result.Error.Equals(Errors.Errors.Cancelled));
            MockAnalyticsService.VerifyTrace(Errors.Errors.Cancelled.ToString(), LogSeverity.Error);
        } 

    }
}