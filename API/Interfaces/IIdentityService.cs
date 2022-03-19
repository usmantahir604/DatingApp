using Microsoft.AspNetCore.Identity;

namespace API.Interfaces
{
    public interface IIdentityService
    {
        Task<IdentityResult> CreateUserAsync(string userName, string password);
    }
}
