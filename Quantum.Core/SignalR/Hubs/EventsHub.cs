using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Quantum.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Quantum.Core.SignalR.Hubs
{

    public static class SignalRListIdsHandler
    {
        public static List<string> ConnectedIds = new List<string>();
    }

    [Authorize(LocalApi.PolicyName)]
    public class EventsHub : Hub
    {
        private IUserManagerService _userMgrServ;
        private ILogger<EventsHub> _logger;

        public EventsHub(IUserManagerService userMgrServ, ILogger<EventsHub> logger)
        {
            _userMgrServ = userMgrServ ?? throw new ArgumentNullException(nameof(userMgrServ));
            _logger = logger;
        }
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"SignalR Context.user.identity Name: {Context.User.Identity.Name}");

            _logger.LogInformation($"SignalR Context.user.identity is auth: {Context.User.Identity.IsAuthenticated}");
            var user = await _userMgrServ.GetAuthUser(Context.User.Identity);
           
            if (user != null)
            {
                _logger.LogInformation($"SignalR userConnected: {user.Id}");
                await Groups.AddToGroupAsync(Context.ConnectionId, user.Id);
                SignalRListIdsHandler.ConnectedIds.Add(user.Id);
                await base.OnConnectedAsync();
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            var user = await _userMgrServ.GetAuthUser(Context.User.Identity);
            if (user != null)
            {
                _logger.LogInformation($"SignalR userDisconnected: {user.Id}");
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, user.Id);
                SignalRListIdsHandler.ConnectedIds.Remove(user.Id);
                await base.OnDisconnectedAsync(exception);
            } 
        }
   
        public Task Send(string data)
        {
            return Clients.All.SendAsync("Send", data);
        }
    }
}
