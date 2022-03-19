using API.DAL.User.Models;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UsersController : BaseAPIController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<AppUserModel>>> GetUsers()
        {
            var result = await _userService.GetAppUsers();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUserModel>> GetUser(int id)
        {
            var result = await _userService.GetAppUser(id);
            return Ok(result);
        }
    }
}
