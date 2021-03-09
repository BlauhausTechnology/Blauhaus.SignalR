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
        public SignalRClientMockBuilder Where_HandleAsync_returns<TCommand>(Response response)
        {
            Mock.Setup(x => x.HandleAsync(It.IsAny<TCommand>())).ReturnsAsync(response);
            return this;
        }
        public SignalRClientMockBuilder Where_HandleAsync_fails<TCommand>(Error error)
        {
            Mock.Setup(x => x.HandleAsync<TCommand>(It.IsAny<TCommand>())).ReturnsAsync(Response.Failure(error));
            return this;
        }
         
        public void VerifyHandleAsync<TCommand>(Expression<Func<TCommand, bool>> predicate)
        {
            Mock.Verify(x => x.HandleAsync(It.Is(predicate)));
        }
         
    }
}