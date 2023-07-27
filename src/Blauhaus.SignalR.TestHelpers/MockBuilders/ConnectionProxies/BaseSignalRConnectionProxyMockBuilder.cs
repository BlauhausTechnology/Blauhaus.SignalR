using Blauhaus.TestHelpers.MockBuilders;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using System;
using Blauhaus.SignalR.Client.Connection;
using Blauhaus.SignalR.Client.Connection.Proxy;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.ConnectionProxies;

public abstract class BaseSignalRConnectionProxyMockBuilder<TBuilder> : BaseMockBuilder<TBuilder, ISignalRConnectionProxy> where TBuilder : BaseSignalRConnectionProxyMockBuilder<TBuilder>
{
    protected BaseSignalRConnectionProxyMockBuilder()
    {

        MockToken = new Mock<IDisposable>();

    }

    public TBuilder Where_InvokeAsync_returns<TDto>(TDto response)
    {
        Mock.Setup(x => x.InvokeAsync<TDto>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(response);
        Mock.Setup(x => x.InvokeAsync<TDto>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>())).ReturnsAsync(response);
        return (TBuilder)this;
    }

    public TBuilder Where_InvokeAsync_throws<TDto>(Exception e)
    {
        Mock.Setup(x => x.InvokeAsync<TDto>(It.IsAny<string>(), It.IsAny<object>())).ThrowsAsync(e);
        Mock.Setup(x => x.InvokeAsync<TDto>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>())).ThrowsAsync(e);
        return (TBuilder)this;
    }


    public void Raise_ClientConnectionStateChange(HubConnectionState state, Exception? exception = null)
    {
        Mock.Raise(x => x.StateChanged += null, new ClientConnectionStateChangeEventArgs(state, exception));
    }
    public Mock<IDisposable> MockToken { get; }
}