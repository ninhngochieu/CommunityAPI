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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MessageController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> CreateMessage(CreateMessageDto messageDto)
        {
            var userName = User.GetUserName();

            if (userName == messageDto.RecipientUsername.ToLower())
                return BadRequestResponse("Bạn không thể gửi thư cho chính mình!");

            var sender = await _unitOfWork.UserRepository.GetUserAsync(userName);
            var recipient = await _unitOfWork.UserRepository.GetUserAsync(messageDto.RecipientUsername);

            if (recipient is null) return NotFoundResponse("Không tìm thấy người nhận");

            var message = new Message
            {
                SenderUser = sender,
                SenderUsername = sender.UserName,
                RecipientUser = recipient,
                RecipientUsername = recipient.UserName,
                Content = messageDto.Content,
            };

            _unitOfWork.MessageRepository.AddMessage(message);

            if (await _unitOfWork.Complete()) return OkResponse(_mapper.Map<MessageDto>(message));

            return BadRequestResponse("Có lỗi khi gửi tin nhắn");
        }

        [HttpGet]
        public async Task<ActionResult> GetMessageForUser([FromQuery] MessageParams @params)
        {
            @params.Username = User.GetUserName();
            var messages = await _unitOfWork.MessageRepository.GetMessageForUser(@params);
            
            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);
            return OkResponse(messages);
        }

        [HttpGet("Thread/{username}")]
        public async Task<ActionResult> GetMessageThread(string username)
        {
            var currentUserName = User.GetUserName();
            return OkResponse(await _unitOfWork.MessageRepository.GetMessageThread(currentUserName, username));
        }

        [HttpDelete("{guid}")]
        public async Task<ActionResult> DeleteMessage(Guid guid)
        {
            var userName = User.GetUserName();
            var message = await _unitOfWork.MessageRepository.GetMessage(guid);
            if (message.SenderUser.UserName != userName && message.RecipientUser.UserName != userName)
            {
                //Nếu người gửi ko phải là ana và người nhận cũng phải là ana
                return UnauthorizedResponse("Bạn không có quyền xoá tin nhắn này");
            }

            if (message.SenderUser.UserName == userName)
            {
                message.SenderDeleted = true;
            }

            if (message.RecipientUser.UserName == userName)
            {
                message.RecipientDeleted = true;
            }

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _unitOfWork.MessageRepository.DeleteMessage(message);
            }

            if (await _unitOfWork.Complete())
            {
                return OkResponse("Đã xoá tin nhắn");
            }

            return BadRequestResponse("Có lỗi xảy ra khi xoá tin nhắn");
        }
    }
    
}