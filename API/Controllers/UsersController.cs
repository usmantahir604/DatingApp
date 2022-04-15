using API.DAL.User.Models;
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

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<ApplicationUserModel>>> GetUsers()
        {
            var result = await _userService.GetApplicationUsers();
            return Ok(result);
        }

        [HttpGet("{username}")]
        
        public async Task<ActionResult<ApplicationUserModel>> GetUser(string username)
        {
            var result = await _userService.GetApplicationUser(username);
            return Ok(result);
        }

        [HttpPut]
        public async Task<ActionResult<ApplicationUserModel>> UpdateUser(UpdateApplicationUserModel model)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _userService.GetUserByUsernameAsync(username);
            _mapper.Map(model,user);
            _userService.Update(user);
            if(await _userService.SaveAllAsync())
            return NoContent();
            return BadRequest("Failed to update user");
        }
    }
}
