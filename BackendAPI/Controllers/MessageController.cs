using System;
using System.Threading.Tasks;
using AutoMapper;
using BackendAPI.DTO;
using BackendAPI.Extentions;
using BackendAPI.Models;
using BackendAPI.Modules;
using BackendAPI.Repository.Interface;
using BackendAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers
{
    [Authorize]
    public class MessageController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessageController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> CreateMessage(CreateMessageDto messageDto)
        {
            var userName = User.GetUserName();

            if (userName == messageDto.RecipientUsername.ToLower())
                return BadRequestResponse("Bạn không thể gửi thư cho chính mình!");

            var sender = await _userRepository.GetUserAsync(userName);
            var recipient = await _userRepository.GetUserAsync(messageDto.RecipientUsername);

            if (recipient is null) return NotFoundResponse("Không tìm thấy người nhận");

            var message = new Message
            {
                SenderUser = sender,
                SenderUsername = sender.UserName,
                RecipientUser = recipient,
                RecipientUsername = recipient.UserName,
                Content = messageDto.Content,
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) return OkResponse(_mapper.Map<MessageDto>(message));

            return BadRequestResponse("Có lỗi khi gửi tin nhắn");
        }

        [HttpGet]
        public async Task<ActionResult> GetMessageForUser([FromQuery] MessageParams @params)
        {
            @params.Username = User.GetUserName();
            var messages = await _messageRepository.GetMessageForUser(@params);
            
            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);
            return OkResponse(messages);
        }

        [HttpGet("Thread/{username}")]
        public async Task<ActionResult> GetMessageThread(string username)
        {
            var currentUserName = User.GetUserName();
            return OkResponse(await _messageRepository.GetMessageThread(currentUserName, username));
        }
    }
    
}