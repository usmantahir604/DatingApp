using API.Common.Models;
using API.DAL.User.Models;

namespace API.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<AppUserModel>> GetAppUsers();
        Task<AppUserModel> GetAppUser(int id);
        Task<Response> CreateUserAsync(CreateUserModel model);
        Task<Response> LoginUserAsync(LoginUserModel model);
    }
}
