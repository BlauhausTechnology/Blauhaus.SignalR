using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blauhaus.SignalR.Abstractions.Sync;
using Blauhaus.SignalR.Client;
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
        
        private readonly List<Func<SyncResponse<MyDto>, Task>> _syncHandlers = new();
        public Mock<IDisposable> AllowMockSync()
        {
            var mockToken = new Mock<IDisposable>();

            Mock.Setup(x => x.Subscribe(It.IsAny<string>(), It.IsAny<Func<SyncResponse<MyDto>, Task>>()))
                .Callback((string methodName, Func<SyncResponse<MyDto>, Task> handler) =>
                {
                    _syncHandlers.Add(handler);
                }).Returns(mockToken.Object);

            return mockToken;
        }
        public async Task PublishMockSyncAsync(SyncResponse<MyDto> dto)
        {
            foreach (var handler in _syncHandlers)
            {
                await handler.Invoke(dto);
            }
        }
        
        
        private readonly List<Func<MyDto, Task>> _connectHandlers = new();
        public Mock<IDisposable> AllowMockConnect()
        {
            var mockToken = new Mock<IDisposable>();

            Mock.Setup(x => x.Subscribe(It.IsAny<string>(), It.IsAny<Func<MyDto, Task>>()))
                .Callback((string methodName, Func<MyDto, Task> handler) =>
                {
                    _connectHandlers.Add(handler);
                }).Returns(mockToken.Object);

            return mockToken;
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