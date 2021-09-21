using System;
using System.Linq;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.Domain.Abstractions.Sync;
using Newtonsoft.Json;

namespace Blauhaus.SignalR.Abstractions.Sync
{
    public class SerializedDtoBatch
    {
        [JsonConstructor]
        public SerializedDtoBatch(
            int currentDtoCount, 
            int remainingDtoCount,
            long batchLastModifiedTicks,
            string serializedDtos)
        {
            SerializedDtos = serializedDtos;
            RemainingDtoCount = remainingDtoCount;
            CurrentDtoCount = currentDtoCount;
            BatchLastModifiedTicks = batchLastModifiedTicks;
        }
        public int CurrentDtoCount { get; }
        public int RemainingDtoCount { get; }
        public long BatchLastModifiedTicks { get; }
        public string SerializedDtos { get; }

        public static SerializedDtoBatch Create<TDto, TId>(DtoBatch<TDto, TId> dtoBatch) 
            where TDto : IClientEntity<TId> where TId : IEquatable<TId>
        {
            var serializedDtos = JsonConvert.SerializeObject(dtoBatch.Dtos);
            var batchLastModified = dtoBatch.Dtos.OrderByDescending(x => x.ModifiedAtTicks).Select(x => x.ModifiedAtTicks).FirstOrDefault();

            return new SerializedDtoBatch(
                dtoBatch.CurrentDtoCount,
                dtoBatch.RemainingDtoCount,
                batchLastModified,
                serializedDtos);
        }

        public DtoBatch<TDto, TId> Extract<TDto, TId>() 
            where TDto : IClientEntity<TId> where TId : IEquatable<TId>
        {
            var dtos = JsonConvert.DeserializeObject<TDto[]>(SerializedDtos) ?? Array.Empty<TDto>();

            return new DtoBatch<TDto, TId>(dtos, RemainingDtoCount);
        }
    }
}