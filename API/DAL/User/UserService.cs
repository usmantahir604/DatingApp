using API.Common.Models;
using API.DAL.User.Models;
using API.Database;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.User
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ILogger _logger;

        public UserService(IMapper mapper, DatabaseContext databaseContext, IIdentityService identityService, ILogger<UserService> logger)
        {
            _mapper = mapper;
            _databaseContext = databaseContext;
            _identityService = identityService;
            _logger = logger;
        }

        public async Task<AppUserModel> GetAppUser(int id)
        {
            var user = await _databaseContext.AppUsers.FindAsync(id);
            var result = _mapper.Map<AppUserModel>(user);
            return result;
        }

        public async Task<IEnumerable<AppUserModel>> GetAppUsers()
        {
            var users = await _databaseContext.AppUsers.ToListAsync();
            var result = _mapper.Map<IEnumerable<AppUserModel>>(users);
            return result;
        }

        public async Task<Response> CreateUserAsync(CreateUserModel model)
        {
            var result = await _identityService.CreateUserAsync(model.Email, model.Password);
            if (!result.Succeeded)
            {
                _logger.LogError("User not created successfully");
                return new Response
                {
                    IsSuccess = false,
                    Message = "User not created successfully",
                    Errors = result.Errors.Select(x => x.Description).ToList()
                };
            }
            return new Response { IsSuccess = true, Message="Account created successfully" };
        }

        public async Task<Response> LoginUserAsync(LoginUserModel model)
        {
            var user = await _identityService.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogError("No user exist with current email address");
                return new Response
                {
                    IsSuccess = false,
                    Message = "Invalid username or password"
                };
            }
            var passwordCheck = await _identityService.CheckPasswordAsync(user, model.Password);
            if(!passwordCheck)
                return new Response { IsSuccess = false, Message = "Invalid username or password" };

            return new Response { IsSuccess = true, Message = "Logged in successfully" };
        }
    }
}
