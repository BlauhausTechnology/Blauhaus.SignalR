using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Client;
using Blauhaus.SignalR.Tests._Base;
using Microsoft.AspNetCore.SignalR.Client;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.SignalRConnectionTests
{
    public class SubscribeAsyncTests : BaseSignalRClientTest<SignalRConnection>
    {
        private List<SignalRConnectionState> _stateChanges;
        private Func<SignalRConnectionState, Task> _handler;

        public override void Setup()
        {
            base.Setup();

            _stateChanges = new List<SignalRConnectionState>();
            _handler = (connectionState) =>
            {
                _stateChanges.Add(connectionState);
                return Task.CompletedTask;
            };
        }


        [Test]
        public async Task WHEN_connection_starts_reconnecting_due_to_exception_SHOULD_log_warning_and_publish_state()
        {
            //Arrang
            var exception = new Exception("foo");
            await Sut.SubscribeAsync(_handler);
            
            //Act
            MockSignalRConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Reconnecting, exception);
            
            //Assert
            MockAnalyticsService.VerifyTrace($"SignalR client hub Reconnecting due to exception: {exception.Message}", LogSeverity.Warning);
            Assert.That(_stateChanges[1], Is.EqualTo(SignalRConnectionState.Reconnecting));
        }
        
        [Test]
        public async Task WHEN_connection_disconnects_due_to_exception_SHOULD_log_warning_and_publish_state()
        {
            //Arrang
            var exception = new Exception("foo");
            await Sut.SubscribeAsync(_handler);
            
            //Act
            MockSignalRConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Disconnected, exception);
            
            //Assert
            MockAnalyticsService.VerifyTrace($"SignalR client hub Disconnected due to exception: {exception.Message}", LogSeverity.Warning);
            Assert.That(_stateChanges[1], Is.EqualTo(SignalRConnectionState.Disconnected));
        }
        
        [Test]
        public async Task WHEN_connection_disconnects_without_exception_SHOULD_log_warning_and_publish_state()
        {
            //Arrang
            await Sut.SubscribeAsync(_handler);
            
            //Act
            MockSignalRConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Disconnected);
            
            //Assert
            MockAnalyticsService.VerifyTrace($"SignalR client hub Disconnected");
            Assert.That(_stateChanges[1], Is.EqualTo(SignalRConnectionState.Disconnected));
        }
        
        [Test]
        public async Task WHEN_connection_is_connected_after_being_disconnected_SHOULD_trace_and_publish_Connected_state()
        {
            //Arrang
            await Sut.SubscribeAsync(_handler);
            MockSignalRConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Disconnected);
            
            //Act
            MockSignalRConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Connected);
            
            //Assert
            MockAnalyticsService.VerifyTrace($"SignalR client hub Connected");
            Assert.That(_stateChanges[2], Is.EqualTo(SignalRConnectionState.Connected));
        }
        
        
        [Test]
        public async Task WHEN_connection_is_connected_after_reconnecting_SHOULD_trace_and_publish_Reconnected_state()
        {
            //Arrang
            await Sut.SubscribeAsync(_handler);
            MockSignalRConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Reconnecting);
            
            //Act
            MockSignalRConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Connected);
            
            //Assert
            MockAnalyticsService.VerifyTrace($"SignalR client hub Reconnected");
            Assert.That(_stateChanges[2], Is.EqualTo(SignalRConnectionState.Reconnected));
        }
    }
}