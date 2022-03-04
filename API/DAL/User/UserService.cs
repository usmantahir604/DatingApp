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

        public UserService(IMapper mapper, DatabaseContext databaseContext)
        {
            _mapper = mapper;
            _databaseContext = databaseContext;
        }

        public async Task<AppUserModel> GetUser(int id)
        {
            var user = await _databaseContext.AppUsers.FindAsync(id);
            var result = _mapper.Map<AppUserModel>(user);
            return result;
        }

        public async Task<IEnumerable<AppUserModel>> GetUsers()
        {
            var users = await _databaseContext.AppUsers.ToListAsync();
            var result = _mapper.Map<IEnumerable<AppUserModel>>(users);
            return result;
        }
    }
}
