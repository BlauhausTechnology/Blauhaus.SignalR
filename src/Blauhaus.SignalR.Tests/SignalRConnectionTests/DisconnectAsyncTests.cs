using System.Threading.Tasks;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Tests.SignalRConnectionTests.Base;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.SignalRConnectionTests
{
    public class DisconnectAsyncTests: BaseSignalRConnectionTest
    {
        [Test]
        public async Task SHOULD_stop_connection()
        {
            //Act
            await Sut.DisconnectAsync();
            
            //Assert
            MockSignalRConnectionProxy.Mock.Verify(x => x.StopAsync());
        }
        
        [Test]
        public async Task SHOULD_log()
        {
            //Act
            await Sut.DisconnectAsync();
            
            //Assert
            MockAnalyticsService.VerifyTrace("SignalR connection disconnecting on request");
        }
        
        [Test]
        public async Task SHOULD_notify_Disconnecting()
        {
            //Arrange
            await Sut.SubscribeAsync(Handler);
            
            //Act
            await Sut.DisconnectAsync();
            
            //Assert
            Assert.That(StateChanges[1], Is.EqualTo(SignalRConnectionState.Disconnecting));
            Assert.That(StateChanges[2], Is.EqualTo(SignalRConnectionState.Disconnected));
        }
    }
}