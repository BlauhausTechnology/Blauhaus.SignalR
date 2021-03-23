using System.Threading.Tasks;
using Blauhaus.SignalR.Tests.Base;
using Blauhaus.SignalR.Tests.Client.SignalRClientTests.Base;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.Client.SignalRClientTests
{
    public class InitializeAllClientsAsyncTests : BaseSignalRClientTest
    {
        [Test]
        public async Task SHOULD_initialize_all_in_Registry()
        {
            //Act
            await Sut.InitializeAllClientsAsync();
            
            //Assert
            SignalRClientRegistry.Mock.Verify(x => x.InitializeAllClientsAsync());
        }
    }
}