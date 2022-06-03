using API.DAL.Message.Models;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub :Hub
    {
        private readonly IMessageService _messageService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public MessageHub(IMessageService messageService, IMapper mapper, IUserService userService)
        {
            _messageService = messageService;
            _mapper = mapper;
            _userService = userService;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext=Context.GetHttpContext();
            var otherUser=httpContext.Request.Query["user"].ToString();
            var groupName=GetGroupName(Context.User.GetUsername(),otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var messages = _messageService.GetMessagesThread(Context.User.GetUsername(), otherUser);

            await Clients.Group(groupName).SendAsync("RecieveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageModel createMessageModel)
        {
            var username = Context.User.GetUsername();
            if (username == createMessageModel.RecipientUsername.ToLower())
               throw new HubException("You cannot send message to yourself");
            var sender = await _userService.GetUserByUsernameAsync(username);
            var recipient = await _userService.GetUserByUsernameAsync(createMessageModel.RecipientUsername);
            if (recipient == null) throw new HubException("Not found user");
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageModel.Content
            };

            _messageService.AddMessage(message);

            if (await _messageService.SaveAllAsync())
            {
                var group=GetGroupName(sender.UserName, recipient.UserName);
                await Clients.Group(group).SendAsync("NewMessage",_mapper.Map<MessageModel>(message));
            }
        }
        private string GetGroupName(string caller, string other)
        {
            var stringCompare=string.CompareOrdinal(caller, other)<0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
