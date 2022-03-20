
using API.Common.Models;
using API.DAL.User.Models;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountController :  BaseAPIController
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost(nameof(Register))]
        public async Task<ActionResult<Response>> Register(CreateUserModel model)
        {
            var result = await _userService.CreateUserAsync(model);
            return Ok(result);
        }

        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login([FromBody] LoginUserModel model)
        {
            var result = await _userService.LoginUserAsync(model);
            return Ok(result);
        }
    }
}
