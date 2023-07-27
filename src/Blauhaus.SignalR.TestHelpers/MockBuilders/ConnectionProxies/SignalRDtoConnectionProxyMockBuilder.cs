using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;

namespace Blauhaus.SignalR.TestHelpers.MockBuilders.ConnectionProxies;


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