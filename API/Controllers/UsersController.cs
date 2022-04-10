using API.DAL.User.Models;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseAPIController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
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
    }
}
