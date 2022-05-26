using API.DAL.User.Models;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user,string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser> FindByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUser> FindByUserNameAsync(string userName)
        {
            return  await _userManager.Users.Include(x=>x.Photos).SingleOrDefaultAsync(x => x.UserName == userName);
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<List<ApplicationUser>> GetUsersWithRoles()
        {
            var users = await _userManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).OrderBy(x=>x.UserName).ToListAsync();
            return users;
        }

        public async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            var result= await _userManager.GetRolesAsync(user);
            return (List<string>)result;
        }
    }

   
}
