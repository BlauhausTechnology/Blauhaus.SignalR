using Blauhaus.Domain.Abstractions.CommandHandlers;
using Blauhaus.Domain.Abstractions.Sync;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRSyncDtoClient<TDto> : ISignalRDtoClient<TDto>, ICommandHandler<IDtoBatch<TDto>, DtoSyncCommand>
    {
        
    }
}