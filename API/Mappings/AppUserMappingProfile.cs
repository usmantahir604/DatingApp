using API.DAL.User.Models;
using API.Entities;
using AutoMapper;

namespace API.Mappings
{
    public class AppUserMappingProfile : Profile
    {
        public AppUserMappingProfile()
        {
            CreateMap<AppUser, AppUserModel>().ReverseMap();
           
        }
    }
}
