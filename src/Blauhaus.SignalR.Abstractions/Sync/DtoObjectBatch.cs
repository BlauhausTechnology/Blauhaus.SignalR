using System;
using System.Collections.Generic;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.Domain.Abstractions.Sync;
using Newtonsoft.Json;

namespace Blauhaus.SignalR.Client.Clients
{
    public class DtoObjectBatch
    {
        [JsonConstructor]
        public DtoObjectBatch(
            IReadOnlyList<object> dtoObjects, 
            int remainingDtoCount)
        {
            DtoObjects = dtoObjects;
            RemainingDtoCount = remainingDtoCount;
        }

        public IReadOnlyList<object> DtoObjects { get; }
        public int RemainingDtoCount { get; }

        
    }
}