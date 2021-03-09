using System;
using System.Linq.Expressions;
using Blauhaus.Common.TestHelpers.MockBuilders;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.SignalR.TestHelpers
{
    public class SignalRConnectionMockBuilder : BaseAsyncPublisherMockBuilder<SignalRConnectionMockBuilder, ISignalRConnection, SignalRConnectionState>
    {
        public SignalRConnectionMockBuilder Where_HandleAsync_returns<TCommand>(Response response)
        {
            Mock.Setup(x => x.HandleAsync(It.IsAny<TCommand>())).ReturnsAsync(response);
            return this;
        }
        public SignalRConnectionMockBuilder Where_HandleAsync_fails<TCommand>(Error error)
        {
            Mock.Setup(x => x.HandleAsync<TCommand>(It.IsAny<TCommand>())).ReturnsAsync(Response.Failure(error));
            return this;
        }


        public SignalRConnectionMockBuilder Where_HandleAsync_returns<TDto, TCommand>(Response<TDto> response)
        {
            Mock.Setup(x => x.HandleAsync<TDto, TCommand>(It.IsAny<TCommand>())).ReturnsAsync(response);
            return this;
        }
        public SignalRConnectionMockBuilder Where_HandleAsync_fails<TDto, TCommand>(Error error)
        {
            Mock.Setup(x => x.HandleAsync<TDto, TCommand>(It.IsAny<TCommand>())).ReturnsAsync(Response.Failure<TDto>(error));
            return this;
        }

        public SignalRConnectionMockBuilder Where_HandleAsync_returns<TDto, TCommand>(TDto response)
        {
            Mock.Setup(x => x.HandleAsync<TDto, TCommand>(It.IsAny<TCommand>())).ReturnsAsync(Response.Success(response));
            return this;
        }


        public void VerifyHandleAsync<TCommand>(Expression<Func<TCommand, bool>> predicate)
        {
            Mock.Verify(x => x.HandleAsync(It.Is(predicate)));
        }

        public void VerifyHandleAsync<TDto, TCommand>(Expression<Func<TCommand, bool>> predicate)
        {
            Mock.Verify(x => x.HandleAsync<TDto, TCommand>(It.Is(predicate)));
        }
    }
}