using Blauhaus.SignalR.Client.Clients;
using Blauhaus.SignalR.Tests.Base;
using Blauhaus.SignalR.Tests.TestObjects;
using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.Client.SignlRDtoClientTests
{
    public class DtoPublishedTests : BaseSignalRClientTest<SignalRDtoClient<MyDto, Guid>>
    {
        private MyDto _dto = null!;

        public override void Setup()
        {
            base.Setup();

            _dto = new MyDto();
        }

        [Test]
        public async Task IF_hub_invocation_succeeds_SHOULD_invoke_handlers()
        { 
            //Arrange
            await Sut.InitializeAsync();

            //Act
            await MockSignalRConnectionProxy.MockPublishDtoAsync(_dto);

            //Assert
            MockMyDtoHandler.Mock.Verify(x => x.HandleAsync(_dto));
        }

        [Test]
        public async Task SHOULD_notify_Subscribers()
        {
            //Arrange
            MyDto? incomingDto = null;
            await Sut.SubscribeAsync(x =>
            {
                incomingDto = x;
                return Task.CompletedTask;
            });

            //Act
            await MockSignalRConnectionProxy.MockPublishDtoAsync(_dto);

            //Assert
            Assert.That(incomingDto!.Id, Is.EqualTo(_dto.Id));
        }
        
        [Test]
        public async Task SHOULD_not_notify_ex_Subscribers()
        {
            //Arrange
            MyDto? incomingDto = null;
            var token = await Sut.SubscribeAsync(x =>
            {
                incomingDto = x;
                return Task.CompletedTask;
            });
            token.Dispose();

            //Act
            await MockSignalRConnectionProxy.MockPublishDtoAsync(_dto);

            //Assert
            Assert.That(incomingDto, Is.Null);
        }
		
    }
}