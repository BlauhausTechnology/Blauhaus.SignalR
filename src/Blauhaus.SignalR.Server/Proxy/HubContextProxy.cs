using System;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Server;
using Microsoft.AspNetCore.SignalR;

namespace Blauhaus.SignalR.Server.Proxy
{
    public class HubContextProxy<THub> : IHubContextProxy where THub : Hub
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IHubContext<THub> _hubContext;

        public HubContextProxy(
            IAnalyticsService analyticsService,
            IHubContext<THub> hubContext)
        {
            _analyticsService = analyticsService;
            _hubContext = hubContext;
        }
         
        public async Task<Response> PublishDtoAsync<TDto>(string connectiondId, TDto dto)
        {
            try
            {
                var methodName = $"Publish{typeof(TDto).Name}Async";
                _analyticsService.Debug($"Publishing {typeof(TDto).Name} to user with connection Id {connectiondId} as {methodName}");
                
                await _hubContext.Clients.Client(connectiondId).SendAsync(methodName, dto);
            }
            catch (Exception e)
            {
                return _analyticsService.LogExceptionResponse(this, e, Errors.Errors.Unexpected(e.Message));
            }
            return Response.Success();
        }
    }
}