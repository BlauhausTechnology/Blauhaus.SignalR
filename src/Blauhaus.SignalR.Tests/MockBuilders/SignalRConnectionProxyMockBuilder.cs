using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Responses;
using Blauhaus.SignalR.Client;
using Blauhaus.SignalR.Client._Ioc;
using Blauhaus.SignalR.Tests.TestObjects;
using Blauhaus.TestHelpers.MockBuilders;
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
        
        private readonly List<Func<MyDto, Task>> _handlers = new List<Func<MyDto, Task>>();
        public Mock<IDisposable> AllowMockSubscriptions()
        {
            var mockToken = new Mock<IDisposable>();

            Mock.Setup(x => x.Subscribe(It.IsAny<string>(), It.IsAny<Func<MyDto, Task>>()))
                .Callback((string methodName, Func<MyDto, Task> handler) =>
                {
                    _handlers.Add(handler);
                }).Returns(mockToken.Object);

            return mockToken;
        }
        public async Task PublishMockSubscriptionAsync(MyDto dto)
        {
            foreach (var handler in _handlers)
            {
                await handler.Invoke(dto);
            }
        }
    }

}