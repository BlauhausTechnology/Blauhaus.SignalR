using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.SignalR.Client
{
    public interface ISignalRConnectionProxy
    {
        Task<TDto> InvokeAsync<TDto>(string methodName, object parameter);
        Task<TDto> InvokeAsync<TDto>(string methodName, object parameter1, object parameter2);
        Task InvokeAsync(string methodName, object parameter);
        Task InvokeAsync(string methodName, object parameter1, object parameter2);

        IDisposable Subscribe<TDto>(string methodName, Func<TDto, Task> handler);
        
        event EventHandler<ClientConnectionStateChangeEventArgs>? StateChanged;
        HubConnectionState CurrentState { get; }
        string ConnectionId { get; }
        Task StopAsync(CancellationToken token);
        ValueTask DisposeAsync();
    }
}