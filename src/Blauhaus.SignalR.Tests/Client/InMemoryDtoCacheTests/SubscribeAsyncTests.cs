using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Blauhaus.SignalR.Tests.Client.InMemoryDtoCacheTests.Base;
using Blauhaus.SignalR.Tests.TestObjects;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.Client.InMemoryDtoCacheTests
{
    public class SubscribeAsyncTests : BaseInMemoryDtoCacheTest
    {
         
        [Test]
        public async Task IF_Dto_With_Id_exists_in_cache_SHOULD_update_only_new_subscriber()
        {
            //Arrange
            Sut.Cache.Add(DtoOne.Id, DtoOne);
            var existingSubscriberReceivedDtos = new List<MyDto>();
            await Sut.SubscribeAsync(d =>
            {
                existingSubscriberReceivedDtos.Add(d);
                return Task.CompletedTask;
            }, DtoOne.Id);
            Assert.That(existingSubscriberReceivedDtos.Count, Is.EqualTo(1));
            existingSubscriberReceivedDtos.Clear();
            
            //Act
            var newSubscriberReceivedDtos = new List<MyDto>();
            await Sut.SubscribeAsync(d =>
            {
                newSubscriberReceivedDtos.Add(d);
                return Task.CompletedTask;
            }, DtoOne.Id);
            
            //Assert
            Assert.That(existingSubscriberReceivedDtos.Count, Is.EqualTo(0));
            Assert.That(newSubscriberReceivedDtos.Count, Is.EqualTo(1));
        }
         
    }
}