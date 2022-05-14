using API.DAL.Like.Models;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikeService
    {
        Task<UserLike> GetUserLike(string sourceUserId, string likedUserId);
        Task<ApplicationUser> GetUserWithLikes(string userId);
        Task<PagedList<LikeModel>> GetUserLikes(LikesParams likesParams);
    }
}
