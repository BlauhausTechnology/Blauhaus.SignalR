using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders
{
    public class SignalRClientMockBuilder<TDto, TSubscribeCommand> : BaseMockBuilder<SignalRClientMockBuilder<TDto, TSubscribeCommand>, ISignalRClient<TDto, TSubscribeCommand>>
    {
        private readonly List<Func<TDto, Task>> _handlers = new List<Func<TDto, Task>>();

        public Mock<IDisposable> Where_SubscribeAsync_publishes_immediately(TDto update)
        {
            var mockToken = new Mock<IDisposable>();

            Mock.Setup(x => x.SubscribeAsync(It.IsAny<TSubscribeCommand>(), It.IsAny<Func<TDto, Task>>()))
                .Callback((TSubscribeCommand command, Func<TDto, Task> handler) =>
                {
                    handler.Invoke(update);
                }).ReturnsAsync(Response.Success(mockToken.Object));

            return mockToken;
        }

        public Mock<IDisposable> Where_SubscribeAsync_publishes_sequence(IEnumerable<TDto> updates)
        {
            var mockToken = new Mock<IDisposable>();
            var queue = new Queue<TDto>(updates);
            
            Mock.Setup(x => x.SubscribeAsync(It.IsAny<TSubscribeCommand>(), It.IsAny<Func<TDto, Task>>()))
                .Callback((TSubscribeCommand command, Func<TDto, Task> handler) =>
                {
                    handler.Invoke(queue.Dequeue());
                }).ReturnsAsync(Response.Success(mockToken.Object));

            return mockToken;
        }
        
        public Mock<IDisposable> AllowMockSubscriptions()
        {
            var mockToken = new Mock<IDisposable>();
            
            Mock.Setup(x => x.SubscribeAsync(It.IsAny<TSubscribeCommand>(), It.IsAny<Func<TDto, Task>>()))
                .Callback((TSubscribeCommand command, Func<TDto, Task> handler) =>
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
        
        public SignalRClientMockBuilder<TDto, TSubscribeCommand> Where_HandleCommandAsync_returns<TCommand>(TDto payload)
        {
            Mock.Setup(x => x.HandleCommandAsync(It.IsAny<TCommand>()))
                .ReturnsAsync(Response.Success(payload));
            return this;
        }
        
        public SignalRClientMockBuilder<TDto, TSubscribeCommand> Where_HandleCommandAsync_returns<TCommand>(List<TDto> payloads)
        {
            var queue = new Queue<Response<TDto>>();
            foreach (var payload in payloads)
            {
                queue.Enqueue(Response.Success(payload));
            }
            Mock.Setup(x => x.HandleCommandAsync(It.IsAny<TCommand>()))
                .ReturnsAsync(queue.Dequeue);
            return this;
        }
        
        public SignalRClientMockBuilder<TDto, TSubscribeCommand> Where_HandleCommandAsync_returns_result<TCommand>(Response<TDto> payload)
        {
            Mock.Setup(x => x.HandleCommandAsync(It.IsAny<TCommand>()))
                .ReturnsAsync(payload);
            return this;
        }
        
        public SignalRClientMockBuilder<TDto, TSubscribeCommand> Where_HandleCommandAsync_returns_fail<TCommand>(Error error)
        {
            Mock.Setup(x => x.HandleCommandAsync(It.IsAny<TCommand>()))
                .ReturnsAsync(Response.Failure<TDto>(error));
            return this;
        }
        
        public SignalRClientMockBuilder<TDto, TSubscribeCommand> Where_HandleCommandAsync_throws<TCommand>(Exception exception)
        {
            Mock.Setup(x => x.HandleCommandAsync(It.IsAny<TCommand>()))
                .ThrowsAsync(exception);
            return this;
        }
        public void Verify_HandleAsync<TCommand>(Expression<Func<TCommand, bool>> predicate, int times = 1)
        {
            Mock.Verify(x => x.HandleCommandAsync(It.Is(predicate)), Times.Exactly(times));
        }
         
    }
}