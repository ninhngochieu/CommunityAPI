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

namespace BackendAPI.Services.Implement
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
            return await _context.Messages.FindAsync(guid);
        }

        public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
            var queryable = _context.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();
            queryable = messageParams.Container switch
            {
                "inbox" => queryable.Where(u => u.RecipientUser.UserName == messageParams.Username), //Tìm những mail được gửi cho mình, có cùng id với mình
                "outbox" => queryable.Where(u => u.SenderUser.UserName == messageParams.Username), // Tìm những mail mình gửi cho người ta
                _=> queryable.Where(u=>u.RecipientUser.UserName == messageParams.Username && u.DateRead == null) // Tìm những mail người ta gửi cho mình mà chưa đọc
            };

            var messageDtos = queryable.AsNoTracking().ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messageDtos, messageParams.PageNumber,
                messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages//Lấy luồng mail
                .Include(u=>u.SenderUser).ThenInclude(p=>p.Photos)
                .Include(u=>u.RecipientUser).ThenInclude(p=>p.Photos)
                .Where(m => m.RecipientUser.UserName == currentUsername && m.SenderUser.UserName == recipientUsername || 
                            m.RecipientUser.UserName == recipientUsername && m.SenderUser.UserName == currentUsername)
                .OrderBy(m=>m.MessageSent)
                .ToListAsync();

            // Tìm mail mà người nhận là mình, chưa đọc, và mình chat với người này trong luồng tin nhắn. Người kia gửi cho mình thì mình mới thấy
            var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUser.UserName == currentUsername).ToList();
            if (unreadMessages.Any())
            {
                unreadMessages.ForEach(m =>
                {
                    m.DateRead = DateTime.Now;
                });
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }


        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}