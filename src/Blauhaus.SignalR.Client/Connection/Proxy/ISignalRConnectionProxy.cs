using System;
using System.Threading.Tasks;
using Blauhaus.Common.Abstractions;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.SignalR.Client.Connection.Proxy
{
    public interface ISignalRConnectionProxy : IAsyncInitializable

    {
    Task<TDto> InvokeAsync<TDto>(string methodName, object parameter);
    Task<TDto> InvokeAsync<TDto>(string methodName, object parameter1, object parameter2);
    Task InvokeAsync(string methodName, object parameter);
    Task InvokeAsync(string methodName, object parameter1, object parameter2);

    IDisposable Subscribe<TDto>(string methodName, Func<TDto, Task> handler);

    event EventHandler<ClientConnectionStateChangeEventArgs>? StateChanged;
    HubConnectionState CurrentState { get; }
    string ConnectionId { get; }
    Task StopAsync();
    ValueTask DisposeAsync();
    }
}