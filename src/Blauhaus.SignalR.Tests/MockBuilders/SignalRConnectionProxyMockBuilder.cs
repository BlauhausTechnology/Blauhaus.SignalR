using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.SignalR.Client.Connection;
using Blauhaus.SignalR.Client.Connection.Proxy;
using Blauhaus.SignalR.Tests.TestObjects;
using Blauhaus.TestHelpers.MockBuilders;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;

namespace Blauhaus.SignalR.Tests.MockBuilders
{
    public class SignalRConnectionProxyMockBuilder : BaseMockBuilder<SignalRConnectionProxyMockBuilder, ISignalRConnectionProxy>
    {
        private readonly List<Func<MyDto, Task>> _connectHandlers = new();

        public SignalRConnectionProxyMockBuilder()
        {
            var mockToken = new Mock<IDisposable>();

            Mock.Setup(x => x.Subscribe(It.IsAny<string>(), It.IsAny<Func<MyDto, Task>>()))
                .Callback((string methodName, Func<MyDto, Task> handler) =>
                {
                    _connectHandlers.Add(handler);
                }).Returns(mockToken.Object);

            MockToken = mockToken;
        }

        public Mock<IDisposable> MockToken { get; }

        public SignalRConnectionProxyMockBuilder Where_InvokeAsync_returns<TDto>(TDto response)
        {
            Mock.Setup(x => x.InvokeAsync<TDto>(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(response);
            Mock.Setup(x => x.InvokeAsync<TDto>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>())).ReturnsAsync(response);
            return this;
        }

        public SignalRConnectionProxyMockBuilder Where_InvokeAsync_throws<TDto>(Exception e)
        {
            Mock.Setup(x => x.InvokeAsync<TDto>(It.IsAny<string>(), It.IsAny<object>())).ThrowsAsync(e);
            Mock.Setup(x => x.InvokeAsync<TDto>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>())).ThrowsAsync(e);
            return this;
        }

        public SignalRConnectionProxyMockBuilder Where_InvokeAsync_throws(Exception e)
        {
            Mock.Setup(x => x.InvokeAsync(It.IsAny<string>(), It.IsAny<object>())).ThrowsAsync(e);
            Mock.Setup(x => x.InvokeAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>())).ThrowsAsync(e);
            return this;
        }

        public void Raise_ClientConnectionStateChange(HubConnectionState state, Exception? exception = null)
        {
            Mock.Raise(x => x.StateChanged += null, new ClientConnectionStateChangeEventArgs(state, exception));
        }
         
        public async Task MockPublishDtoAsync(MyDto dto)
        {
            foreach (var handler in _connectHandlers)
            {
                await handler.Invoke(dto);
            }
        }
    }

}