using AutoFixture;
using Blauhaus.Common.Utils.Extensions;
using Blauhaus.SignalR.Tests.Base;
using Blauhaus.SignalR.Tests.TestObjects;

namespace Blauhaus.SignalR.Tests.Client.InMemoryDtoCacheTests.Base
{
    public class BaseInMemoryDtoCacheTest : BaseSignalRClientTest<TestInMemoryDtoCache>
    {
        protected MyDto DtoOne;
        protected MyDto DtoTwo;
        protected MyDto DtoThree;

        public override void Setup()
        {
            base.Setup();

            DtoOne = MyFixture.Create<MyDto>().With(x => x.Name, "Bob");
            DtoTwo = MyFixture.Create<MyDto>().With(x => x.Name, "Frank");
            DtoThree = MyFixture.Create<MyDto>().With(x => x.Name, "Bill");
            
        }

    }
}