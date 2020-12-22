using System;
using System.Threading.Tasks;
using Blauhaus.Common.Utils.Contracts;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Subscriptions;

namespace Blauhaus.SignalR.Abstractions.Client
{
    public interface ISignalRClient<TDto>
    {
        Task<Response<TDto>> HandleAsync<TCommand>(TCommand command) where TCommand : notnull;
        Task<Response<IDisposable>> SubscribeAsync(Func<TDto, Task> handler, DtoSubscription? dtoSubscription = null);
    }
}