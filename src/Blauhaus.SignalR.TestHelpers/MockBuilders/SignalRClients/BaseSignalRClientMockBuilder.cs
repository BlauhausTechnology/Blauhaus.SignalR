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
    
    public abstract class BaseSignalRClientMockBuilder<TBuilder, TMock, TDto> : BaseAsyncPublisherMockBuilder<TBuilder, TMock, TDto>
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
         
    }
    
}