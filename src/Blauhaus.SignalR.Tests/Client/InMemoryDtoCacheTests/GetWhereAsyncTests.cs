using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Blauhaus.SignalR.Tests.Client.InMemoryDtoCacheTests.Base;
using Blauhaus.SignalR.Tests.TestObjects;
using NUnit.Framework;

namespace Blauhaus.SignalR.Tests.Client.InMemoryDtoCacheTests
{
    public class GetWhereAsyncTests : BaseInMemoryDtoCacheTest
    {

        [Test]
        public async Task IF_Dtos_exist_SHOULD_return_matching()
        {
            //Arrange
            Sut.Cache.Add(DtoOne.Id, DtoOne);
            Sut.Cache.Add(DtoTwo.Id, DtoTwo);
            Sut.Cache.Add(DtoThree.Id, DtoThree);
            
            //Act
            var result = await Sut.GetWhereAsync(x => x.Name.StartsWith("B"));
            
            //Assert
            Assert.That(result[0], Is.EqualTo(DtoOne));
            Assert.That(result[1], Is.EqualTo(DtoThree));
        }
        

        [Test]
        public async Task IF_Dtos_exist_but_none_match_SHOULD_return_empty_list()
        {
            //Arrange
            Sut.Cache.Add(DtoOne.Id, DtoOne);
            Sut.Cache.Add(DtoTwo.Id, DtoTwo);
            Sut.Cache.Add(DtoThree.Id, DtoThree);
            
            //Act
            var result = await Sut.GetWhereAsync(x => x.Name.StartsWith("XXX"));
            
            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }
        
        [Test]
        public async Task IF_no_Dtos_exist_SHOULD_return_empty_list()
        {
            //Act
            var result = await Sut.GetAllAsync();
            
            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }
         
    }
}