using System;
using System.Linq.Expressions;
using Blauhaus.Common.TestHelpers.MockBuilders;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Client;
using Moq;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders
{
    public class SignalRClientMockBuilder : BaseAsyncPublisherMockBuilder<SignalRClientMockBuilder, ISignalRClient, SignalRConnectionState>
    {
        public SignalRClientMockBuilder Where_HandleAsync_returns<TCommand>(Response response) where TCommand : notnull
        {
            Mock.Setup(x => x.HandleVoidCommandAsync(It.IsAny<TCommand>())).ReturnsAsync(response);
            return this;
        }
        public SignalRClientMockBuilder Where_HandleAsync_fails<TCommand>(Error error) where TCommand : notnull
        {
            Mock.Setup(x => x.HandleVoidCommandAsync<TCommand>(It.IsAny<TCommand>())).ReturnsAsync(Response.Failure(error));
            return this;
        }
        public Error Where_HandleAsync_fails<TCommand>() where TCommand : notnull
        {
            var error = Error.Create(Guid.NewGuid().ToString());
            Mock.Setup(x => x.HandleVoidCommandAsync<TCommand>(It.IsAny<TCommand>())).ReturnsAsync(Response.Failure(error));
            return error;
        }
         
        public void VerifyHandleAsync<TCommand>(Expression<Func<TCommand, bool>> predicate) where TCommand : notnull
        {
            Mock.Verify(x => x.HandleVoidCommandAsync(It.Is(predicate)));
        }

        public SignalRClientMockBuilder Where_HandleAsync_succeeds<TCommand, TResponse>(TResponse response) where TCommand : notnull
        {
            Mock.Setup(x => x.HandleCommandAsync<TCommand, TResponse>(It.IsAny<TCommand>())).ReturnsAsync(Response.Success(response));
            return this;
        }
        public SignalRClientMockBuilder Where_HandleAsync_succeeds<TCommand, TResponse>(Func<TResponse> response) where TCommand : notnull
        {
            Mock.Setup(x => x.HandleCommandAsync<TCommand, TResponse>(It.IsAny<TCommand>()))
                .ReturnsAsync(()=> Response.Success(response.Invoke()));
            return this;
        }
        public SignalRClientMockBuilder Where_HandleAsync_fails<TCommand, TResponse>(Error error) where TCommand : notnull
        {
            Mock.Setup(x => x.HandleCommandAsync<TCommand, TResponse>(It.IsAny<TCommand>())).ReturnsAsync(Response.Failure<TResponse>(error));
            return this;
        }
        public Error Where_HandleAsync_fails<TCommand, TResponse>() where TCommand : notnull
        {
            var error = Error.Create(Guid.NewGuid().ToString());
            Mock.Setup(x => x.HandleCommandAsync<TCommand, TResponse>(It.IsAny<TCommand>())).ReturnsAsync(Response.Failure<TResponse>(error));
            return error;
        }
         
        public void VerifyHandleAsync<TCommand, TResponse>(Expression<Func<TCommand, bool>> predicate) where TCommand : notnull
        {
            Mock.Verify(x => x.HandleCommandAsync<TCommand, TResponse>(It.Is(predicate)));
        }
         
    }
}