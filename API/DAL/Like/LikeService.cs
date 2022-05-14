using API.DAL.Like.Models;
using API.Database;
using API.Entities;
using API.Extensions;
using API.Helpers;
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

        public async Task<PagedList<LikeModel>> GetUserLikes(LikesParams likesParams)
        {
            var users = _databaseContext.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _databaseContext.Likes.AsQueryable();

            if (likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.LikedUser);
            }

            if (likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers= users.Select(user => new LikeModel
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            });
            return await PagedList<LikeModel>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<ApplicationUser> GetUserWithLikes(string userId)
        {
            return await _databaseContext.Users.Include(x=>x.LikedUsers).Where(x=>x.Id == userId).FirstOrDefaultAsync();
        }
    }
}
