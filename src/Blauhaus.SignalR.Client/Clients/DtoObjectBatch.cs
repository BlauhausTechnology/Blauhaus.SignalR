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

        public DtoBatch<TDto, TId> ToDtoBatch<TDto, TId>() 
            where TDto : IClientEntity<TId> 
            where TId : IEquatable<TId>
        {
            var dtos = new TDto[DtoObjects.Count];
            for (var i = 0; i < dtos.Length; i++)
            {
                dtos[i] = (TDto)DtoObjects[i];
            }

            return new DtoBatch<TDto, TId>(dtos, RemainingDtoCount);
        }
         
    }
}