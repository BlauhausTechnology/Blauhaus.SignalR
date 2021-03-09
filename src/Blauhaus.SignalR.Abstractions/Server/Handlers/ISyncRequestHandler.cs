using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.SignalR.Abstractions.Sync;

namespace Blauhaus.SignalR.Abstractions.Server.Handlers
{
    public interface ISyncRequestHandler<TDto> : IConnectedUserCommandHandler<SyncResponse<TDto>, SyncRequest> 
        where TDto : IClientEntity
    {
        
    }
}