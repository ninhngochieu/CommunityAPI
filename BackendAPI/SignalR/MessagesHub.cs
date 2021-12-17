using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IHubContext<PresenceHub> _hubContext;
        private readonly PresenceTracker _tracker;
        private readonly IUnitOfWork _unitOfWork;
        private const string _newMessageReceived = "NewMessageReceived";
        private const string _newMessage = "NewMessage";
        private const string _receiveMessageThread = "ReceiveMessageThread";

        public MessagesHub(IMessageRepository messageRepository, IMapper mapper, IUserRepository userRepository,
            IHubContext<PresenceHub> hubContext, PresenceTracker tracker, IUnitOfWork unitOfWork)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _hubContext = hubContext;
            _tracker = tracker;
            _unitOfWork = unitOfWork;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var otherUser = httpContext.Request.Query["user"].ToString();

            var groupName = GetGroupName(Context.User.GetUserName(), otherUser); // Cách để vị trí của 2 người chat không bao giờ bị thay đổi

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await AddToGroup(Context, groupName);

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

            var groupName = GetGroupName(sender.UserName, recipient.UserName);

            var messageGroup = await _messageRepository.GetMessageGroup(groupName);
            // Trong 1 luồng chat sẽ có 2 người. Nếu connection mà thiếu người nhận --> Họ đã off
            if (messageGroup.Connections.Any(x=>x.Username == recipient.UserName)) // Nếu group message này có bất kỳ ai là người nhận
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connectionForUser = await _tracker.GetConnectionForUser(recipient.UserName); // Lấy tất cả connection id

                if (connectionForUser is not null)
                {
                    // Gửi tất cả thông tin xuống Clients theo connection Id ngoại trừ mình
                    await _hubContext.Clients.Clients(connectionForUser).SendAsync(_newMessageReceived, new
                    {
                        username = sender.UserName,
                        knownAs = sender.KnownAs
                    });
                }
            }
            
            _messageRepository.AddMessage(message);

            if (await _unitOfWork.Complete())
            {
                // var groupName = GetGroupName(sender.UserName, recipient.UserName);

                await Clients.Group(groupName).SendAsync(_newMessage, _mapper.Map<MessageDto>(message));
            };
            
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await RemoveFromMessageGroup(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        private async Task<bool> AddToGroup(HubCallerContext caller, string groupName)
        {
            // Thêm connection hiện tại vô group
            var messageGroup = await _messageRepository.GetMessageGroup(groupName);

            var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());

            if (messageGroup is null)
            {
                messageGroup = new Group(groupName);
                _messageRepository.AddGroup(messageGroup);
            }
            
            messageGroup.Connections.Add(connection);

            return await _unitOfWork.Complete();
        }

        private async Task RemoveFromMessageGroup(string connectionId)
        {
            var connection = await _messageRepository.GetConnection(connectionId);
            
            _messageRepository.RemoveConnection(connection);

            await _unitOfWork.Complete();
        }
    }
}