using System.Collections.Generic;
using Blauhaus.Domain.Abstractions.Entities;
using Newtonsoft.Json;

namespace Blauhaus.SignalR.Abstractions.Sync
{
    public class SyncResponse<TDto> where TDto : IClientEntity
    {
        [JsonConstructor]
        public SyncResponse(TDto[]? dtos = null)
        {
            Dtos = dtos ?? new TDto[0];
        }

        public TDto[] Dtos { get; }
    }
}