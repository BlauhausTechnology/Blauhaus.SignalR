using System;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Extensions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Errors;
using Blauhaus.Responses;
using Blauhaus.SignalR.Abstractions.Server;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Blauhaus.SignalR.Server.Proxy
{
    public class HubContextProxy<THub> : IHubContextProxy where THub : Hub
    {
        private readonly IAnalyticsLogger<THub> _logger;
        private readonly IHubContext<THub> _hubContext;

        public HubContextProxy(
            IAnalyticsLogger<THub> logger,
            IHubContext<THub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }
         
        public async Task<Response> PublishDtoAsync<TDto>(string connectiondId, TDto dto)
        {
            try
            {
                _logger.LogTrace("Publishing Dto {DtoType} to connection {ConnectionId}", typeof(TDto).Name, connectiondId);
                await _hubContext.Clients.Client(connectiondId).SendAsync($"Publish{typeof(TDto).Name}Async", dto);
            }
            catch (Exception e)
            {
                return _logger.LogErrorResponse(Error.Unexpected(e.Message), e);
            }
            return Response.Success();
        }
    }
}