using System;
using System.Threading.Tasks;
using BackendAPI.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BackendAPI.SignalR
{
    [Authorize]
    public class PresenceHub: Hub 
    {
        private readonly PresenceTracker _tracker;
        private const string _getOnlineUsers = "GetOnlineUsers";
        private const string _userIsOffline = "UserIsOffline";
        private const string _userIsOnline = "UserIsOnline";

        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }
        
        public override async Task OnConnectedAsync()
        {
            await _tracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);
            await Clients.Others.SendAsync(_userIsOnline, Context.User.GetUserName());

            var currentOnlineUsers = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync(_getOnlineUsers, currentOnlineUsers);
            
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _tracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);
            await Clients.Others.SendAsync(_userIsOffline, Context.User.GetUserName());
            
            var currentOnlineUsers = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync(_getOnlineUsers, currentOnlineUsers);
            
            await base.OnDisconnectedAsync(exception);
            
        }
    }
}