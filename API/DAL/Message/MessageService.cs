using API.DAL.Message.Models;
using API.Database;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace API.DAL.Message
{
    public class MessageService : IMessageService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;
        public MessageService(DatabaseContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }

        public void AddMessage(Entities.Message message)
        {
            _databaseContext.Messages.Add(message);
        }

        public void DeleteMessage(Entities.Message message)
        {
            _databaseContext.Messages.Remove(message);
        }

        public async Task<Entities.Message> GetMessage(int id)
        {
            return await _databaseContext.Messages.FindAsync(id);
        }

        public Task<PagedList<MessageModel>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _databaseContext.Messages.OrderByDescending(x=>x.MessageSent).AsQueryable();
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(x=>x.RecipientUsername==messageParams.Username),
                "Outbox" => query.Where(x=>x.SenderUsername==messageParams.Username),
                _ => query.Where(x=>x.RecipientUsername==messageParams.Username && x.DateRead==null)
            };

            var message = query.ProjectTo<MessageModel>(_mapper.ConfigurationProvider);
            return PagedList<MessageModel>.CreateAsync(message,messageParams.PageNumber,messageParams.PageSize);
        }

        public Task<IEnumerable<MessageModel>> GetMessagesThread(string currentUserId, string recipientId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _databaseContext.SaveChangesAsync()>0;
        }
    }
}
