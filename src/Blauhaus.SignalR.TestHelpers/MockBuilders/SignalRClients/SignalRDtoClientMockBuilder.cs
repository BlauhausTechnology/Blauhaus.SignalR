using Blauhaus.Errors;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.TestHelpers.Builders.Base;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using Blauhaus.Responses;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.SignalRClients
{
    public class SignalRDtoClientMockBuilder<TDto> : BaseSignalRDtoClientMockBuilder<SignalRDtoClientMockBuilder<TDto>, ISignalRDtoClient<TDto>, TDto> where TDto : class
    {
    }

     public abstract class BaseSignalRDtoClientMockBuilder<TBuilder, TMock, TDto> : BaseMockBuilder<TBuilder, TMock>
        where TBuilder : BaseSignalRDtoClientMockBuilder<TBuilder, TMock, TDto> 
        where TDto : class
        where TMock : class, ISignalRDtoClient<TDto>
    {
        
        public TBuilder Where_HandleCommandAsync_succeeds<TCommand>(TDto payload) where TCommand : notnull
        {
            Mock.Setup(x => x.HandleCommandAsync(It.IsAny<TCommand>()))
                .ReturnsAsync(Response.Success(payload));
            return (TBuilder) this;
        }
        public TBuilder Where_HandleCommandAsync_succeeds<TCommand>(Func<TDto> payload) where TCommand : notnull
        {
            Mock.Setup(x => x.HandleCommandAsync(It.IsAny<TCommand>()))
                .ReturnsAsync(()=> Response.Success(payload.Invoke()));
            return (TBuilder) this;
        }
        public TBuilder Where_HandleCommandAsync_succeeds<TCommand>(IBuilder<TDto> payload) where TCommand : notnull
        {
            Mock.Setup(x => x.HandleCommandAsync(It.IsAny<TCommand>()))
                .ReturnsAsync(()=> Response.Success(payload.Object));
            return (TBuilder) this;
        }

        public TBuilder Where_HandleCommandAsync_returns<TCommand>(List<TDto> payloads) where TCommand : notnull
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
        
        public TBuilder Where_HandleCommandAsync_returns<TCommand>(Response<TDto> payload) where TCommand : notnull
        {
            Mock.Setup(x => x.HandleCommandAsync(It.IsAny<TCommand>()))
                .ReturnsAsync(payload);
            return (TBuilder) this;
        }
        
        public TBuilder Where_HandleCommandAsync_fails<TCommand>(Error error) where TCommand : notnull
        {
            Mock.Setup(x => x.HandleCommandAsync(It.IsAny<TCommand>()))
                .ReturnsAsync(Response.Failure<TDto>(error));
            return (TBuilder) this;
        }        
        public Error Where_HandleCommandAsync_fails<TCommand>() where TCommand : notnull
        {
            var error = Error.Create(Guid.NewGuid().ToString());
            Mock.Setup(x => x.HandleCommandAsync(It.IsAny<TCommand>()))
                .ReturnsAsync(Response.Failure<TDto>(error));
            return error;
        }
        
        public TBuilder Where_HandleCommandAsync_throws<TCommand>(Exception exception) where TCommand : notnull
        {
            Mock.Setup(x => x.HandleCommandAsync(It.IsAny<TCommand>()))
                .ThrowsAsync(exception);
            return (TBuilder) this;
        }
        public void VerifyHandleCommandAsync<TCommand>(Expression<Func<TCommand, bool>> predicate, int times = 1) where TCommand : notnull
        {
            Mock.Verify(x => x.HandleCommandAsync(It.Is(predicate)), Times.Exactly(times));
        }
         
    }
}