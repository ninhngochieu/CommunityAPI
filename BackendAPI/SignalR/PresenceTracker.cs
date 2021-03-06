using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendAPI.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> _onlineUsers = new Dictionary<string, List<string>>();

        public Task UserConnected(string username, string connectionId)
        {
            lock (_onlineUsers) // Check user với các connection id
            {
                if (_onlineUsers.ContainsKey(username))
                {
                    _onlineUsers[username].Add(connectionId);
                }
                else
                {
                    _onlineUsers.Add(username, new List<string> {connectionId});
                }
            }
            return Task.CompletedTask;
        }

        public Task UserDisconnected(string username, string connectionId)
        {
            lock (_onlineUsers)
            {
                if (!_onlineUsers.ContainsKey(username)) return Task.CompletedTask;

                _onlineUsers[username].Remove(connectionId);

                if (_onlineUsers[username].Count == 0) // Check số lượng connection Id có trong list
                {
                    _onlineUsers.Remove(username);
                }
            }
            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;

            lock (_onlineUsers)
            {
                onlineUsers = _onlineUsers
                    .OrderBy(k => k.Key)
                    .Select(k => k.Key)// Select tên của user
                    .ToArray();
            }
            
            return Task.FromResult(onlineUsers);
        }
        
        public Task<List<string>> GetConnectionForUser(string username)
        {
            List<string> connectionId;
            lock (_onlineUsers)
            {
                connectionId = _onlineUsers.GetValueOrDefault(username);
            }

            return Task.FromResult(connectionId);
        }
    }
}