using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackendAPI.DTO;
using BackendAPI.Models;
using BackendAPI.Modules;
using BackendAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Repository.Implement
{
    public class MessageRepository: IMessageRepository
    {
        private readonly CommunityContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(CommunityContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddGroup(Group @group)
        {
            _context.Groups.Add(group);
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(Guid guid)
        {
            return await _context.Messages.Include(u=>u.SenderUser).Include(u=>u.RecipientUser).SingleOrDefaultAsync(x=>x.Id == guid);
        }

        public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
            var queryable = _context.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();
            queryable = messageParams.Container switch
            {
                "inbox" => queryable.Where(u => u.RecipientUser.UserName == messageParams.Username && u.RecipientDeleted == false), //T??m nh???ng mail ???????c g???i cho m??nh, c?? c??ng id v???i m??nh
                "outbox" => queryable.Where(u => u.SenderUser.UserName == messageParams.Username && u.SenderDeleted== false), // T??m nh???ng mail m??nh g???i cho ng?????i ta
                _=> queryable.Where(u=>u.RecipientUser.UserName == messageParams.Username &&u.RecipientDeleted == false && u.DateRead == null) // T??m nh???ng mail ng?????i ta g???i cho m??nh m?? ch??a ?????c
            };

            var messageDtos = queryable.AsNoTracking().ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messageDtos, messageParams.PageNumber,
                messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages//L???y lu???ng mail
                // .Include(u=>u.SenderUser).ThenInclude(p=>p.Photos)
                // .Include(u=>u.RecipientUser).ThenInclude(p=>p.Photos)
                .Where(m => m.RecipientUser.UserName == currentUsername && m.SenderUser.UserName == recipientUsername && m.RecipientDeleted == false || 
                            m.RecipientUser.UserName == recipientUsername && m.SenderUser.UserName == currentUsername && m.SenderDeleted == false)
                .OrderBy(m=>m.MessageSent)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            // T??m mail m?? ng?????i nh???n l?? m??nh, ch??a ?????c, v?? m??nh chat v???i ng?????i n??y trong lu???ng tin nh???n. Ng?????i kia g???i cho m??nh th?? m??nh m???i th???y
            var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUsername == currentUsername).ToList();
            if (unreadMessages.Any())
            {
                unreadMessages.ForEach(m =>
                {
                    m.DateRead = DateTime.UtcNow;
                });
                // await _context.SaveChangesAsync();
            }

            // return _mapper.Map<IEnumerable<MessageDto>>(messages);
            return messages;
        }


        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}