using API.Common.Models;
using API.DAL.User.Models;
using API.Database;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.DAL.User
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ILogger _logger;

        public UserService(IMapper mapper, DatabaseContext databaseContext, IIdentityService identityService, ITokenGenerator tokenGenerator, ILogger<UserService> logger)
        {
            _mapper = mapper;
            _databaseContext = databaseContext;
            _identityService = identityService;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
        }


        
        public async Task<ApplicationUserModel> GetApplicationUser(string username)
        {
            //Project to not call those properties which are not part of VM and Join is not allow with projection
            var user = await _databaseContext.Users.
                ProjectTo<ApplicationUserModel>(_mapper.ConfigurationProvider)
                //.Include(p => p.Photos)
                .SingleOrDefaultAsync(x=>x.Username== username);
            
            //var result = _mapper.Map<ApplicationUserModel>(user);
            return user;
        }

        public async Task<IEnumerable<ApplicationUserModel>> GetApplicationUsers()
        {
            var users = await _databaseContext.Users
                //.Include(x=>x.Photos)
                .ProjectTo<ApplicationUserModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
            //var result = _mapper.Map<IEnumerable<ApplicationUserModel>>(users);
            return users;
        }

        public async Task<Response<AuthenticateUserModel>> CreateUserAsync(CreateUserModel model)
        {
            var result = await _identityService.CreateUserAsync(model.UserName, model.Password);
            if (!result.Succeeded)
            {
                _logger.LogError("User not created successfully");
                //return new Response<AuthenticateUserModel>
                //{
                //    IsSuccess = false,
                //    Message = "User not created successfully",
                //    Errors = result.Errors.Select(x => x.Description).ToList()
                //};
                throw new FluentValidation.ValidationException("User not created successfully");
            }
            var identityUser = await _identityService.FindByUserNameAsync(model.UserName);
            return await _tokenGenerator.GenerateUserToken(identityUser);
        }

        public async Task<Response<AuthenticateUserModel>> LoginUserAsync(LoginUserModel model)
        {
            var user = await _identityService.FindByUserNameAsync(model.UserName);
            if (user == null)
            {
                _logger.LogError("No user exist with current email address");
                throw new FluentValidation.ValidationException("Invalid username or password");
            }
            var passwordCheck = await _identityService.CheckPasswordAsync(user, model.Password);
            if (!passwordCheck)
                throw new FluentValidation.ValidationException("Invalid username or password");

            return await _tokenGenerator.GenerateUserToken(user);
        }

        public async Task<Response<AuthenticateUserModel>> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = _tokenGenerator.GetClaimsPrincipalFromToken(token);
            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(expiryDateUnix);

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            var storedRefreshToken = _databaseContext.RefreshTokens.SingleOrDefault(x => x.Token == new Guid(refreshToken));

            var validation = IsTokenValid(expiryDateTimeUtc, storedRefreshToken, jti);
            if (!validation.IsSuccess)
            {
                _logger.LogError(validation.Message);
                return validation;
            }

            storedRefreshToken.Used = true;
            await _databaseContext.SaveChangesAsync(new CancellationToken());
            var userId = validatedToken.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _identityService.FindByIdAsync(userId);
            return await _tokenGenerator.GenerateUserToken(user);
        }


        private static Response<AuthenticateUserModel> IsTokenValid(DateTime expiryDateTimeUtc, RefreshToken storedRefreshToken, string jti)
        {
            if (expiryDateTimeUtc > DateTime.UtcNow)
                return new Response<AuthenticateUserModel>
                {
                    IsSuccess = false,
                    Message = "This token is not expired yet"
                };

            if (storedRefreshToken == null)
                return new Response<AuthenticateUserModel>
                {
                    IsSuccess = false,
                    Message = "This refresh token does not exist"
                };

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                return new Response<AuthenticateUserModel>
                {
                    IsSuccess = false,
                    Message = "This refresh token has already expired"
                };

            if (storedRefreshToken.Invalidated)
                return new Response<AuthenticateUserModel>
                {
                    IsSuccess = false,
                    Message = "This refresh token has been invalidated"
                };

            if (storedRefreshToken.Used)
                return new Response<AuthenticateUserModel>
                {
                    IsSuccess = false,
                    Message = "This refresh token has been used"
                };

            if (storedRefreshToken.JwtId != jti)
                return new Response<AuthenticateUserModel>
                {
                    IsSuccess = false,
                    Message = "This refresh token does not match JWT"
                };

            return new Response<AuthenticateUserModel>
            {
                IsSuccess = true
            };
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await _databaseContext.Users.FindAsync(id);
        }

        public async Task<ApplicationUser> GetUserByUsernameAsync(string username)
        {
            return await _databaseContext.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);

        }


        public async Task<string> GetUserGender(string username)
        {
            return await _databaseContext.Users
                .Where(x => x.UserName == username)
                .Select(x => x.Gender).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync()
        {
            return await _databaseContext.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public void Update(ApplicationUser user)
        {
            _databaseContext.Entry(user).State = EntityState.Modified;
        }

    }
}