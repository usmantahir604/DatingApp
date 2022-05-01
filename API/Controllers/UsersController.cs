using API.DAL.User.Models;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseAPIController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public IPhotoService _photoService { get; }

        public UsersController(IUserService userService,IPhotoService photoService, IMapper mapper)
        {
            _userService = userService;
            _photoService = photoService;
            _mapper = mapper;
        }

        [HttpGet()]
        public async Task<ActionResult<PagedList<ApplicationUserModel>>> GetUsers([FromQuery] UserParams userParams)
        {
            var result = await _userService.GetApplicationUsers(userParams);
            Response.AddPaginationHeader(result.CurrentPage, result.PageSize, result.TotalCount, result.TotalPages);
            return Ok(result);
        }

        [HttpGet("{username}",Name = "GetUser")]
        
        public async Task<ActionResult<ApplicationUserModel>> GetUser(string username)
        {
            var result = await _userService.GetApplicationUser(username);
           
            return Ok(result);
        }

        [HttpPut]
        public async Task<ActionResult<ApplicationUserModel>> UpdateUser(UpdateApplicationUserModel model)
        {
            var user = await _userService.GetUserByUsernameAsync(User.GetUsername());
            _mapper.Map(model,user);
            _userService.Update(user);
            if(await _userService.SaveAllAsync())
            return NoContent();
            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoModel>> AddPhoto(IFormFile file)
        {
            var user= await _userService.GetUserByUsernameAsync(User.GetUsername());
            var result = await _photoService.AddPhotoAsync(file);
            if(result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }
            user.Photos.Add(photo);
            if(await _userService.SaveAllAsync())
            {
                //return _mapper.Map<PhotoModel>(photo);
                return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoModel>(photo));
            }
            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<IActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userService.GetUserByUsernameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo.IsMain) return BadRequest("This is already your main photo");
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if(currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;
            if(await _userService.SaveAllAsync())
                return NoContent();
            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            var user = await _userService.GetUserByUsernameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("You cannot delete your main photo");
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error!=null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if(await _userService.SaveAllAsync()) return Ok();
            return BadRequest("Unable to delete photo");
        }
    }
}
