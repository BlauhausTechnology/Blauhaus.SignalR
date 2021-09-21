using System;
using Blauhaus.Domain.Abstractions.CommandHandlers;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.Domain.Abstractions.Sync;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRSyncDtoClient<TDto, TId> : ISignalRDtoClient<TDto>, ICommandHandler<DtoBatch<TDto, TId>, DtoSyncCommand> 
        where TDto : IClientEntity<TId> 
        where TId : IEquatable<TId>
    {
        
    }
}