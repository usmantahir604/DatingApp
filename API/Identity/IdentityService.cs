using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace API.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateUserAsync(string userName, string password)
        {
            var user = new ApplicationUser
            {
                Email = userName,
                UserName = userName
            };
            return await _userManager.CreateAsync(user, password);
        }
    }

   
}
