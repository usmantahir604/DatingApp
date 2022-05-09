using API.DAL.Like.Models;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseAPIController
    {
        private readonly IUserService _userService;
        private readonly ILikeService _likeService;
        public LikesController(IUserService userService, ILikeService likeService)
        {
            _userService = userService;
            _likeService = likeService;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _userService.GetUserByUsernameAsync(username);
            var sourceUser = await _likeService.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();

            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await _likeService.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("You already like this user");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if(await _userService.SaveAllAsync())
                return Ok();
            return BadRequest("Failed to like user");

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeModel>>> GetUserLikes([FromQuery] string predicate)
        {
            var users = await _likeService.GetUserLikes(predicate, User.GetUserId());
            return Ok(users);
        }
    }
}
