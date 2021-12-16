using System;
using System.Threading.Tasks;
using AutoMapper;
using BackendAPI.DTO;
using BackendAPI.Extentions;
using BackendAPI.Models;
using BackendAPI.Repository.Interface;
using BackendAPI.Services.Interface;
using Microsoft.AspNetCore.SignalR;

namespace BackendAPI.SignalR
{
    public class MessagesHub: Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private const string _newMessage = "NewMessage";
        private const string _receiveMessageThread = "ReceiveMessageThread";

        public MessagesHub(IMessageRepository messageRepository,IMapper mapper,IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var otherUser = httpContext.Request.Query["user"].ToString();

            var groupName = GetGroupName(Context.User.GetUserName(), otherUser); // Cách để vị trí của 2 người chat không bao giờ bị thay đổi

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var messages = await _messageRepository.GetMessageThread(Context.User.GetUserName(), otherUser);

            await Clients.Group(groupName).SendAsync(_receiveMessageThread, messages);
        }

        private static string GetGroupName(string caller, string other)
        {
            var compareOrdinal = string.CompareOrdinal(caller, other) < 0;

            return compareOrdinal ? $"{caller}-{other}" : $"{other}-{caller}";
            // throw new NotImplementedException();
        }

        public async Task SendMessage(CreateMessageDto messageDto)
        {
            var userName = Context.User.GetUserName();

            if (userName == messageDto.RecipientUsername.ToLower())
                throw new HubException("Bạn không thể gửi thư cho chính mình!");

            var sender = await _userRepository.GetUserAsync(userName);
            var recipient = await _userRepository.GetUserAsync(messageDto.RecipientUsername);

            if (recipient is null) throw new HubException("Không tìm thấy người nhận");

            var message = new Message
            {
                SenderUser = sender,
                SenderUsername = sender.UserName,
                RecipientUser = recipient,
                RecipientUsername = recipient.UserName,
                Content = messageDto.Content,
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync())
            {
                var groupName = GetGroupName(sender.UserName, recipient.UserName);

                await Clients.Group(groupName).SendAsync(_newMessage, _mapper.Map<MessageDto>(message));
            };
            
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}