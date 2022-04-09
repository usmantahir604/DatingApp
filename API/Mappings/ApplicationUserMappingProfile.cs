using API.DAL.User.Models;
using API.Entities;
using AutoMapper;

namespace API.Mappings
{
    public class ApplicationUserMappingProfile : Profile
    {
        public ApplicationUserMappingProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserModel>();
            CreateMap<Photo, PhotoModel>();
        }
    }
}
