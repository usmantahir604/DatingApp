using API.DAL.Like.Models;
using API.Entities;

namespace API.Interfaces
{
    public interface ILikeService
    {
        Task<UserLike> GetUserLike(string sourceUserId, string likedUserId);
        Task<ApplicationUser> GetUserWithLikes(string userId);
        Task<IEnumerable<LikeModel>> GetUserLikes(string predicate, string userId);
    }
}
