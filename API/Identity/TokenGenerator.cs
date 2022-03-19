using API.Common.Models;
using API.Interfaces;
using Microsoft.Extensions.Options;

namespace API.Identity
{
    public class TokenGenerator: ITokenGenerator
    {
        private readonly JwtSettings _jwtSettings;
        public TokenGenerator(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }
    }
}
