using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.Domain.Abstractions.Entities;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Moq;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.SignalRClients
{

    public class SignalRSyncClientMockBuilder<TDto> : BaseSignalRClientMockBuilder<SignalRSyncClientMockBuilder<TDto>, ISignalRSyncClient<TDto>, TDto> 
        where TDto : class, IClientEntity
    {
        private readonly List<Func<TDto, Task>> _handlers = new List<Func<TDto, Task>>();

        public Mock<IDisposable> Where_SubscribeAsync_publishes_immediately(TDto update)
        {
            var mockToken = new Mock<IDisposable>();

            Mock.Setup(x => x.SyncAsync(It.IsAny<Func<TDto, Task>>()))
                .Callback((Func<TDto, Task> handler) =>
                {
                    handler.Invoke(update);
                }).ReturnsAsync(Response.Success(mockToken.Object));

            return mockToken;
        }

        public Mock<IDisposable> Where_SubscribeAsync_publishes_sequence(IEnumerable<TDto> updates)
        {
            var mockToken = new Mock<IDisposable>();
            var queue = new Queue<TDto>(updates);
            
            Mock.Setup(x => x.SyncAsync(It.IsAny<Func<TDto, Task>>()))
                .Callback((Func<TDto, Task> handler) =>
                {
                    handler.Invoke(queue.Dequeue());
                }).ReturnsAsync(Response.Success(mockToken.Object));

            return mockToken;
        }
        
        public Mock<IDisposable> AllowMockSubscriptions()
        {
            var mockToken = new Mock<IDisposable>();
            
            Mock.Setup(x => x.SyncAsync(It.IsAny<Func<TDto, Task>>()))
                .Callback((Func<TDto, Task> handler) =>
                {
                    _handlers.Add(handler);
                }).ReturnsAsync(Response.Success(mockToken.Object));

            return mockToken;
        }

        public async Task PublishMockSubscriptionAsync(TDto model)
        {
            foreach (var handler in _handlers)
            {
                await handler.Invoke(model);
            }
        }
        
         
    }
}