using System;
using System.Threading;
using Blauhaus.Responses;
using Blauhaus.SignalR.Client;
using Blauhaus.SignalR.Client._Ioc;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.SignalR.Tests.MockBuilders
{
    public class SignalRConnectionProxyMockBuilder : BaseMockBuilder<SignalRConnectionProxyMockBuilder, ISignalRConnectionProxy>
    {


        public SignalRConnectionProxyMockBuilder Where_InvokeAsync_returns<TDto>(TDto response)
        {
            Mock.Setup(x => x.InvokeAsync<TDto>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);
            Mock.Setup(x => x.InvokeAsync<TDto>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);
            return this;
        }

        public SignalRConnectionProxyMockBuilder Where_InvokeAsync_throws<TDto>(Exception e)
        {
            Mock.Setup(x => x.InvokeAsync<TDto>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>())).ThrowsAsync(e);
            Mock.Setup(x => x.InvokeAsync<TDto>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>(), It.IsAny<CancellationToken>())).ThrowsAsync(e);
            return this;
        }

        public SignalRConnectionProxyMockBuilder Where_InvokeAsync_throws(Exception e)
        {
            Mock.Setup(x => x.InvokeAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>())).ThrowsAsync(e);
            Mock.Setup(x => x.InvokeAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>(), It.IsAny<CancellationToken>())).ThrowsAsync(e);
            return this;
        }
    }
}