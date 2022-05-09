using API.DAL.Like.Models;
using API.Database;
using API.Entities;
using API.Interfaces;

namespace API.DAL.Like
{
    public class LikeService : ILikeService
    {
        private readonly DatabaseContext _databaseContext;
        public LikeService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        public Task<UserLike> GetUserLike(string sourceUserId, string likedUserId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LikeModel>> GetUserLikes(string predicate, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> GetUserWithLikes(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
