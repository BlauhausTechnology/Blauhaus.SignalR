using System;
using System.Threading.Tasks;
using Blauhaus.Domain.TestHelpers.MockBuilders.Client.Sync;
using Blauhaus.SignalR.Client.Clients;
using Blauhaus.SignalR.Tests.Base;
using Blauhaus.SignalR.Tests.TestObjects;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.Client.SignalRSyncDtoClientTests
{
    public class DtoPublishedTests : BaseSignalRClientTest<SignalRSyncDtoClient<MyDto, Guid>>
    {
        private MyDto _dto = null!;
        
        protected SyncDtoCacheMockBuilder<MyDto, Guid> MockSyncDtoCache = null!;

        public override void Setup()
        {
            base.Setup();

            _dto = new MyDto();

            
            MockSyncDtoCache = new SyncDtoCacheMockBuilder<MyDto, Guid>();
            AddService(MockSyncDtoCache.Object);
        }

        [Test]
        public async Task IF_hub_invocation_succeeds_SHOULD_invoke_handlers()
        { 
            //Arrange
            await Sut.InitializeAsync();

            //Act
            await MockSignalRConnectionProxy.MockPublishDtoAsync(_dto);

            //Assert
            MockSyncDtoCache.Mock.Verify(x => x.HandleAsync(_dto));
        }
         
        
        [Test]
        public async Task IF_hub_invocation_succeeds_SHOULD_invoke_SyncDtoCache()
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