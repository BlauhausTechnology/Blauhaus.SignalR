using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Blauhaus.SignalR.Tests.Client.InMemoryDtoCacheTests.Base;
using Blauhaus.SignalR.Tests.TestObjects;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.Client.InMemoryDtoCacheTests
{
    public class TryGetOneAsyncTests : BaseInMemoryDtoCacheTest
    {

        [Test]
        public async Task IF_Dto_exists_SHOULD_return()
        {
            //Arrange
            Sut.Cache.Add(DtoThree.Id, DtoThree);
            
            //Act
            var result = await Sut.TryGetOneAsync(DtoThree.Id);
            
            //Assert
            Assert.That(result, Is.EqualTo(DtoThree));
        }
        
        [Test]
        public async Task IF_Dto_does_not_existS_SHOULD_return_null()
        {
            //Arrange
            Sut.Cache.Add(DtoThree.Id, DtoThree);
            
            //Act
            var result = await Sut.TryGetOneAsync(DtoOne.Id);
            
            //Assert
            Assert.That(result, Is.Null);
        }
         
    }
}