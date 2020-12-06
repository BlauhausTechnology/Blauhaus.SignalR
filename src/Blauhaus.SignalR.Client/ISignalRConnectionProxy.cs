using System;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Responses;
using Microsoft.AspNetCore.SignalR.Client;

namespace Blauhaus.SignalR.Client
{
    public interface ISignalRConnectionProxy
    {
        Task<TDto> InvokeAsync<TDto>(string methodName, object parameter, CancellationToken token = default);
        Task<TDto> InvokeAsync<TDto>(string methodName, object parameter1, object parameter2, CancellationToken token = default);
        Task InvokeAsync(string methodName, object parameter, CancellationToken token = default);
        Task InvokeAsync(string methodName, object parameter1, object parameter2, CancellationToken token = default);

        IObservable<TDto> On<TDto>(string methodName);
        event EventHandler<ClientConnectionStateChangeEventArgs>? StateChanged;
        HubConnectionState CurrentState { get; }
        string ConnectionId { get; }
        Task StopAsync(CancellationToken token);
        ValueTask DisposeAsync();
    }
}