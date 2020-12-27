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
         
        public async Task<Response> ConnectAsync<TDto>(string connectiondId, TDto dto)
        {
            try
            {
                await _hubContext.Clients.Client(connectiondId)
                    .SendAsync($"Connect{typeof(TDto).Name}Async", dto);
            }
            catch (Exception e)
            {
                return _analyticsService.LogExceptionResponse(this, e, Errors.Errors.Unexpected(e.Message));
            }
            return Response.Success();
        }
    }
}