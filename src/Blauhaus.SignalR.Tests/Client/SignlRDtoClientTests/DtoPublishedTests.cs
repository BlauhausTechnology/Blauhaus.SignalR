using Blauhaus.SignalR.Client.Clients;
using Blauhaus.SignalR.Tests.Base;
using Blauhaus.SignalR.Tests.TestObjects;
using System;
using System.Threading.Tasks;
using Blauhaus.Domain.TestHelpers.MockBuilders.Client.DtoCaches;
using Blauhaus.SignalR.Tests.Client.SignlRDtoClientTests.Base;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.Client.SignlRDtoClientTests
{
    public class DtoPublishedTests : BaseSignalRDtoClientTest
    {
        private MyDto _dto = null!;

        public override void Setup()
        {
            base.Setup();

            _dto = new MyDto();

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