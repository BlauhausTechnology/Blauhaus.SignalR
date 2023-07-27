using System;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Tests.Client.SignalRClientTests.Base;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.Client.SignalRClientTests
{
    public class SubscribeAsyncTests : BaseSignalRClientTest
    {

        [Test]
        public async Task WHEN_connection_starts_reconnecting_due_to_exception_SHOULD_publish_state()
        {
            //Arrang
            var exception = new Exception("foo");
            await Sut.SubscribeAsync(Handler);
            
            //Act
            MockSignalRDtoConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Reconnecting, exception);
            
            //Assert
            Assert.That(StateChanges[0], Is.EqualTo(SignalRConnectionState.Reconnecting));
        }
        
        [Test]
        public async Task WHEN_connection_disconnects_due_to_exception_SHOULD_publish_state()
        {
            //Arrang
            var exception = new Exception("foo");
            await Sut.SubscribeAsync(Handler);
            
            //Act
            MockSignalRDtoConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Disconnected, exception);
            
            //Assert
            Assert.That(StateChanges[0], Is.EqualTo(SignalRConnectionState.Disconnected));
        }
        
        [Test]
        public async Task WHEN_connection_disconnects_without_exception_SHOULD_log_warning_and_publish_state()
        {
            //Arrang
            await Sut.SubscribeAsync(Handler);
            
            //Act
            MockSignalRDtoConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Disconnected);
            
            //Assert
            Assert.That(StateChanges[0], Is.EqualTo(SignalRConnectionState.Disconnected));
        }
        
        [Test]
        public async Task WHEN_connection_is_connected_after_being_disconnected_SHOULD_trace_and_publish_Connected_state()
        {
            //Arrang
            await Sut.SubscribeAsync(Handler);
            MockSignalRDtoConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Disconnected);
            
            //Act
            MockSignalRDtoConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Connected);
            
            //Assert
            Assert.That(StateChanges[1], Is.EqualTo(SignalRConnectionState.Connected));
        }
        
        
        [Test]
        public async Task WHEN_connection_is_connected_after_reconnecting_SHOULD_trace_and_publish_Reconnected_state()
        {
            //Arrang
            await Sut.SubscribeAsync(Handler);
            MockSignalRDtoConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Reconnecting);
            
            //Act
            MockSignalRDtoConnectionProxy.Raise_ClientConnectionStateChange(HubConnectionState.Connected);
            
            //Assert
            Assert.That(StateChanges[1], Is.EqualTo(SignalRConnectionState.Reconnected));
        }
    }
}