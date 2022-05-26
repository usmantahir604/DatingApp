using API.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.Interfaces
{
    public interface IIdentityService
    {
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<ApplicationUser> FindByIdAsync(string id);
        Task<ApplicationUser> FindByUserNameAsync(string userName);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);

        Task<List<ApplicationUser>> GetUsersWithRoles();
        Task<List<string>> GetUserRoles(ApplicationUser user);
    }
}
