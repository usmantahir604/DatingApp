using API.Common.Models;
using API.DAL.User.Models;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserService
    {
        Task<PagedList<ApplicationUserModel>> GetApplicationUsers(UserParams userParams);
        Task<ApplicationUserModel> GetApplicationUser(string username);
        Task<Response<AuthenticateUserModel>> CreateUserAsync(CreateUserModel model);
        Task<Response<AuthenticateUserModel>> LoginUserAsync(LoginUserModel model);
        Task<Response<AuthenticateUserModel>> RefreshTokenAsync(string token, string refreshToken);
        Task<ApplicationUser> GetUserByIdAsync(int id);
        Task<ApplicationUser> GetUserByUsernameAsync(string username);
        Task<string> GetUserGender(string username);
        Task<IEnumerable<ApplicationUser>> GetUsersAsync();
        Task<List<ApplicationUser>> GetUsersWithRoles();
        Task<List<string>> GetUserRoles(ApplicationUser user);
        void Update(ApplicationUser user);
        Task<bool> SaveAllAsync();
    }
}
