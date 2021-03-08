using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.SignalRClients
{
    
    public abstract class BaseSignalRClientMockBuilder<TBuilder, TMock, TDto> : BaseMockBuilder<TBuilder, TMock>
        where TBuilder : BaseSignalRClientMockBuilder<TBuilder, TMock, TDto> 
        where TDto : class
        where TMock : class, ISignalRClient<TDto>
    {
        
        public TBuilder Where_HandleCommandAsync_returns<TCommand>(TDto payload)
        {
            Mock.Setup(x => x.HandleCommandAsync(It.IsAny<TCommand>()))
                .ReturnsAsync(Response.Success(payload));
            return (TBuilder) this;
        }
        
        public TBuilder Where_HandleCommandAsync_returns<TCommand>(List<TDto> payloads)
        {
            var queue = new Queue<Response<TDto>>();
            foreach (var payload in payloads)
            {
                queue.Enqueue(Response.Success(payload));
            }
            Mock.Setup(x => x.HandleCommandAsync(It.IsAny<TCommand>()))
                .ReturnsAsync(queue.Dequeue);
            return (TBuilder) this;
        }
        
        public TBuilder Where_HandleCommandAsync_returns<TCommand>(Response<TDto> payload)
        {
            Mock.Setup(x => x.HandleCommandAsync(It.IsAny<TCommand>()))
                .ReturnsAsync(payload);
            return (TBuilder) this;
        }
        
        public TBuilder Where_HandleCommandAsync_fails<TCommand>(Error error)
        {
            Mock.Setup(x => x.HandleCommandAsync(It.IsAny<TCommand>()))
                .ReturnsAsync(Response.Failure<TDto>(error));
            return (TBuilder) this;
        }
        
        public TBuilder Where_HandleCommandAsync_throws<TCommand>(Exception exception)
        {
            Mock.Setup(x => x.HandleCommandAsync(It.IsAny<TCommand>()))
                .ThrowsAsync(exception);
            return (TBuilder) this;
        }
        public void VerifyHandleCommandAsync<TCommand>(Expression<Func<TCommand, bool>> predicate, int times = 1)
        {
            Mock.Verify(x => x.HandleCommandAsync(It.Is(predicate)), Times.Exactly(times));
        }
        
        
        private readonly List<Func<TDto, Task>> _connectHandlers = new List<Func<TDto, Task>>();

        public Mock<IDisposable> Where_SubscribeAsync_publishes_immediately(TDto update, Guid? id = null)
        {
            var mockToken = new Mock<IDisposable>();

            if(id == null)
            {
                Mock.Setup(x => x.SubscribeAsync(It.IsAny<Guid>(), It.IsAny<Func<TDto, Task>>()))
                    .Callback((Guid givenId, Func<TDto, Task> handler) =>
                    {
                        _connectHandlers.Add(handler);
                        handler.Invoke(update);
                    }).ReturnsAsync(Response.Success(mockToken.Object));

            }
            else
            {
                Mock.Setup(x => x.SubscribeAsync(id.Value, It.IsAny<Func<TDto, Task>>()))
                    .Callback((Guid givenId, Func<TDto, Task> handler) =>
                    {
                        _connectHandlers.Add(handler);
                        handler.Invoke(update);
                    }).ReturnsAsync(Response.Success(mockToken.Object));

            }
            return mockToken;

        }

        public Mock<IDisposable> Where_SubscribeAsync_publishes_sequence(IEnumerable<TDto> updates, Guid? id = null)
        {
            var mockToken = new Mock<IDisposable>();
            var queue = new Queue<TDto>(updates);

            if (id == null)
            {
                Mock.Setup(x => x.SubscribeAsync(It.IsAny<Guid>(), It.IsAny<Func<TDto, Task>>()))
                    .Callback((Guid givenId, Func<TDto, Task> handler) =>
                    {
                        _connectHandlers.Add(handler);
                        handler.Invoke(queue.Dequeue());
                    }).ReturnsAsync(Response.Success(mockToken.Object));
            }
            else
            {
                Mock.Setup(x => x.SubscribeAsync(id.Value, It.IsAny<Func<TDto, Task>>()))
                    .Callback((Guid givenId, Func<TDto, Task> handler) =>
                    {
                        _connectHandlers.Add(handler);
                        handler.Invoke(queue.Dequeue());
                    }).ReturnsAsync(Response.Success(mockToken.Object));
            }
            return mockToken;
        }
         

        public async Task PublishMockConnectionAsync(TDto model)
        {
            foreach (var handler in _connectHandlers)
            {
                await handler.Invoke(model);
            }
        }
    }
    
}