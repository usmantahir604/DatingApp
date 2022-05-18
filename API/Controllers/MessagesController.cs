using API.DAL.Message.Models;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseAPIController
    {
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public MessagesController(IMessageService messageService, IUserService userService, IMapper mapper)
        {
            _messageService = messageService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageModel>> CreateMesage([FromBody] CreateMessageModel createMessage)
        {
            var username = User.GetUsername();
            if (username == createMessage.RecipientUsername.ToLower())
                return BadRequest("You cannot send message to yourself");
            var sender= await _userService.GetUserByUsernameAsync(username);
            var recipient = await _userService.GetUserByUsernameAsync(createMessage.RecipientUsername);
            if(recipient == null) return NotFound();
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessage.Content
            };

             _messageService.AddMessage(message);

            if(await _messageService.SaveAllAsync()) return Ok(_mapper.Map<MessageModel>(message));
            return BadRequest();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageModel>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username= User.GetUsername();
            var messages = await _messageService.GetMessagesForUser(messageParams);
            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);
            return Ok(messages);
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageModel>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();
            return Ok(await _messageService.GetMessagesThread(currentUsername, username));
        }
    }
}
