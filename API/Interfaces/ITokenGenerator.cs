using API.Common.Models;
using API.DAL.User.Models;
using API.Entities;
using System.Security.Claims;

namespace API.Interfaces
{
    public interface ITokenGenerator
    {
        Task<Response<AuthenticateUserModel>> GenerateUserToken(ApplicationUser applicationUser);
        ClaimsPrincipal GetClaimsPrincipalFromToken(string token);
    }
}
