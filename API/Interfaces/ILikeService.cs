using API.DAL.Like.Models;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikeService
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        Task<ApplicationUser> GetUserWithLikes(int userId);
        Task<PagedList<LikeModel>> GetUserLikes(LikesParams likesParams);
    }
}
