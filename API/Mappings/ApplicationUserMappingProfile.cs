using API.DAL.User.Models;
using API.Entities;
using AutoMapper;

namespace API.Mappings
{
    public class ApplicationUserMappingProfile : Profile
    {
        public ApplicationUserMappingProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserModel>()
                .ForMember(dest=>dest.PhotoUrl, opt=>opt.MapFrom(src=>src.Photos.FirstOrDefault(x=>x.IsMain).Url));
            CreateMap<Photo, PhotoModel>();
        }
    }
}
