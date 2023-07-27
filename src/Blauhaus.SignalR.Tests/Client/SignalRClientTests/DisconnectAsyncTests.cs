using System.Threading.Tasks;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.SignalR.Tests.Client.SignalRClientTests.Base;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.Client.SignalRClientTests
{
    public class DisconnectAsyncTests: BaseSignalRClientTest
    {
        [Test]
        public async Task SHOULD_stop_connection()
        {
            //Act
            await Sut.DisconnectAsync();
            
            //Assert
            MockSignalRDtoConnectionProxy.Mock.Verify(x => x.StopAsync());
        }
        
        [Test]
        public async Task SHOULD_log()
        {
            //Act
            await Sut.DisconnectAsync();
            
            //Assert
            MockLogger.VerifyLog("SignalR connection disconnecting on request");
        }
        
        [Test]
        public async Task SHOULD_notify_Disconnecting()
        {
            //Arrange
            await Sut.SubscribeAsync(Handler);
            
            //Act
            await Sut.DisconnectAsync();
            
            //Assert
            Assert.That(StateChanges[0], Is.EqualTo(SignalRConnectionState.Disconnecting));
            Assert.That(StateChanges[1], Is.EqualTo(SignalRConnectionState.Disconnected));
        }
    }
}