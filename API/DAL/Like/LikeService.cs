using API.DAL.Like.Models;
using API.Database;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.Like
{
    public class LikeService : ILikeService
    {
        private readonly DatabaseContext _databaseContext;
        public LikeService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }
        public async Task<UserLike> GetUserLike(string sourceUserId, string likedUserId)
        {
            return await _databaseContext.Likes.FindAsync(sourceUserId, likedUserId);
        }

        public async Task<IEnumerable<LikeModel>> GetUserLikes(string predicate, string userId)
        {
            var users = _databaseContext.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _databaseContext.Likes.AsQueryable();

            if (predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == userId);
                users = likes.Select(like => like.LikedUser);
            }

            if (predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == userId);
                users = likes.Select(like => like.SourceUser);
            }

            return await users.Select(user => new LikeModel
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            }).ToListAsync();
        }

        public async Task<ApplicationUser> GetUserWithLikes(string userId)
        {
            return await _databaseContext.Users.Include(x=>x.LikedUsers).Where(x=>x.Id == userId).FirstOrDefaultAsync();
        }
    }
}
