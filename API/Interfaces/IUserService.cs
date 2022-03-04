using API.DAL.User.Models;

namespace API.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<AppUserModel>> GetUsers();
        Task<AppUserModel> GetUser(int id);
    }
}
