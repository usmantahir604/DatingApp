using API.DAL.Message.Models;
using API.Database;
using API.Helpers;
using API.Interfaces;
namespace API.DAL.Message
{
    public class MessageService : IMessageService
    {
        private readonly DatabaseContext _databaseContext;
        public MessageService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
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

        public Task<PagedList<MessageModel>> GetMessagesForUser()
        {
            throw new NotImplementedException();
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
