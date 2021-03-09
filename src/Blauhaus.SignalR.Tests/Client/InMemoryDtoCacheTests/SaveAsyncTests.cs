using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Blauhaus.SignalR.Tests.Client.InMemoryDtoCacheTests.Base;
using Blauhaus.SignalR.Tests.TestObjects;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.Client.InMemoryDtoCacheTests
{
    public class SaveAsyncTests : BaseInMemoryDtoCacheTest
    {

        [Test]
        public async Task SHOULD_add_Dto_to_Cache()
        {
            //Act
            await Sut.SaveAsync(DtoOne);
            
            //Assert
            Assert.That(Sut.Cache.First().Value, Is.EqualTo(DtoOne));
        }
        
        [Test]
        public async Task SHOULD_not_add_duplicate()
        {
            //Act
            await Sut.SaveAsync(DtoOne);
            await Sut.SaveAsync(DtoOne);
            
            //Assert
            Assert.That(Sut.Cache.Count, Is.EqualTo(1));
            Assert.That(Sut.Cache.First().Value, Is.EqualTo(DtoOne));
        }
        
        [Test]
        public async Task SHOULD_update_subscribers_to_that_id()
        {
            //Arrange
            var publishedDtos = new List<MyDto>();
            await Sut.SubscribeAsync(d =>
            {
                publishedDtos.Add(d);
                return Task.CompletedTask;
            }, DtoOne.Id);
            
            //Act
            await Sut.SaveAsync(DtoOne);
            
            //Assert
            Assert.That(publishedDtos.Count, Is.EqualTo(1));
            Assert.That(publishedDtos.First(), Is.EqualTo(DtoOne));
        }
        
        [Test]
        public async Task SHOULD_NOT_update_subscribers_to_different_id()
        {
            //Arrange
            var publishedDtos = new List<MyDto>();
            await Sut.SubscribeAsync(d =>
            {
                publishedDtos.Add(d);
                return Task.CompletedTask;
            }, DtoOne.Id);
            
            //Act
            await Sut.SaveAsync(DtoThree);
            
            //Assert
            Assert.That(publishedDtos.Count, Is.EqualTo(0));
        }
    }
}