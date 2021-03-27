using System;
using System.Collections.Generic;
using Blauhaus.SignalR.Client.DtoCache;
using Blauhaus.SignalR.Tests.TestObjects;

namespace Blauhaus.SignalR.Tests.Client.InMemoryDtoCacheTests.Base
{
    public class TestInMemoryDtoCache: InMemoryDtoCache<MyDto, Guid>
    {
        public Dictionary<Guid, MyDto> Cache => CachedDtos;
       
    }
}