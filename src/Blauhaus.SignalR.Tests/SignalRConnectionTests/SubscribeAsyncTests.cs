using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Client;
using Blauhaus.SignalR.Tests.SignalRConnectionTests.Base;
using Microsoft.AspNetCore.SignalR.Client;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.SignalRConnectionTests
{
    public class SubscribeAsyncTests : BaseSignalRConnectionTest
    {
      

        [Test]
        public async Task WHEN_connection_starts_reconnecting_due_to_exception_SHOULD_log_warning_and_publish_state()
        {
            //Arrang
            var exception = new Exception("foo");
            await Sut.SubscribeAsync(Handler);
            
            //Act
            MockSignalRConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Reconnecting, exception);
            
            //Assert
            MockAnalyticsService.VerifyTrace($"SignalR client hub Reconnecting due to exception: {exception.Message}", LogSeverity.Warning);
            Assert.That(StateChanges[1], Is.EqualTo(SignalRConnectionState.Reconnecting));
        }
        
        [Test]
        public async Task WHEN_connection_disconnects_due_to_exception_SHOULD_log_warning_and_publish_state()
        {
            //Arrang
            var exception = new Exception("foo");
            await Sut.SubscribeAsync(Handler);
            
            //Act
            MockSignalRConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Disconnected, exception);
            
            //Assert
            MockAnalyticsService.VerifyTrace($"SignalR client hub Disconnected due to exception: {exception.Message}", LogSeverity.Warning);
            Assert.That(StateChanges[1], Is.EqualTo(SignalRConnectionState.Disconnected));
        }
        
        [Test]
        public async Task WHEN_connection_disconnects_without_exception_SHOULD_log_warning_and_publish_state()
        {
            //Arrang
            await Sut.SubscribeAsync(Handler);
            
            //Act
            MockSignalRConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Disconnected);
            
            //Assert
            MockAnalyticsService.VerifyTrace($"SignalR client hub Disconnected");
            Assert.That(StateChanges[1], Is.EqualTo(SignalRConnectionState.Disconnected));
        }
        
        [Test]
        public async Task WHEN_connection_is_connected_after_being_disconnected_SHOULD_trace_and_publish_Connected_state()
        {
            //Arrang
            await Sut.SubscribeAsync(Handler);
            MockSignalRConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Disconnected);
            
            //Act
            MockSignalRConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Connected);
            
            //Assert
            MockAnalyticsService.VerifyTrace($"SignalR client hub Connected");
            Assert.That(StateChanges[2], Is.EqualTo(SignalRConnectionState.Connected));
        }
        
        
        [Test]
        public async Task WHEN_connection_is_connected_after_reconnecting_SHOULD_trace_and_publish_Reconnected_state()
        {
            //Arrang
            await Sut.SubscribeAsync(Handler);
            MockSignalRConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Reconnecting);
            
            //Act
            MockSignalRConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Connected);
            
            //Assert
            MockAnalyticsService.VerifyTrace($"SignalR client hub Reconnected");
            Assert.That(StateChanges[2], Is.EqualTo(SignalRConnectionState.Reconnected));
        }
    }
}