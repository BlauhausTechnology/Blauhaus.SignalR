using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.SignalR.Client.Connection;
using Blauhaus.SignalR.Client.Connection.Proxy;
using Blauhaus.TestHelpers.MockBuilders;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders;

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

public class SignalRConnectionProxyMockBuilder : BaseSignalRConnectionProxyMockBuilder<SignalRConnectionProxyMockBuilder>
{
}

public class SignalRDtoConnectionProxyMockBuilder<TDto> : BaseSignalRConnectionProxyMockBuilder<SignalRDtoConnectionProxyMockBuilder<TDto>>
{
    private readonly List<Func<TDto, Task>> _connectHandlers = new();

    public SignalRDtoConnectionProxyMockBuilder()
    {
        Mock.Setup(x => x.Subscribe(It.IsAny<string>(), It.IsAny<Func<TDto, Task>>()))
            .Callback((string methodName, Func<TDto, Task> handler) =>
            {
                _connectHandlers.Add(handler);
            }).Returns(MockToken.Object);
    }
    public async Task MockPublishDtoAsync(TDto dto)
    {
        foreach (var handler in _connectHandlers)
        {
            await handler.Invoke(dto);
        }
    }
    
}