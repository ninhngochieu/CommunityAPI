using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackendAPI.DTO;
using BackendAPI.Models;
using BackendAPI.Modules;

namespace BackendAPI.Services.Interface
{
    public interface IMessageRepository
    {
        void AddGroup(Group @group);
        void RemoveConnection(Connection connection);
        Task<Connection> GetConnection(string connectionId);
        Task<Group> GetMessageGroup(string groupName);
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(Guid guid);
        Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams);

        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);
        // Task<bool> SaveAllAsync();
    }

}