using API.Common.Models;
using API.DAL.User.Models;
using API.Database;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Identity
{
    public class TokenGenerator: ITokenGenerator
    {
        private readonly DatabaseContext _databaseContext;
        private readonly JwtSettings _jwtSettings;
        public TokenGenerator(IOptions<JwtSettings> jwtSettings, DatabaseContext databaseContext)
        {
            _jwtSettings = jwtSettings.Value;
            _databaseContext = databaseContext;
        }

        public async Task<Response<AuthenticateUserModel>> GenerateUserToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifeTime),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id.ToString(),
                CreatedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };
            await _databaseContext.RefreshTokens.AddAsync(refreshToken);
            await _databaseContext.SaveChangesAsync();
            return new Response<AuthenticateUserModel>
            {
                IsSuccess = true,
                Message = "User login successfully",
                Result = new AuthenticateUserModel
                {
                    UserName = user.UserName,
                    Token = tokenHandler.WriteToken(token),
                    RefreshToken = refreshToken.Token.ToString()
                }
            };

        }

        public ClaimsPrincipal GetClaimsPrincipalFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = false,
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                };
                var principle = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                return !IsJwtWithValidSecurityAlgorithm(validatedToken) ? null : principle;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken)
                   && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
