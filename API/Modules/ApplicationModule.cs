using API.Mappings;
using AutoMapper;

namespace API.Modules
{
    public static class ApplicationModule
    {
        public static Profile AppUserMappingProfile()
        {
            return new AppUserMappingProfile();
        }

        public static Profile ApplicationUserMappingProfile()
        {
            return new ApplicationUserMappingProfile();
        }
    }
}
