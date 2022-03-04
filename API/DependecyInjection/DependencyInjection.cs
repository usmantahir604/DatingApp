using API.DAL.User;
using API.Interfaces;
using API.Modules;
using AutoMapper;

namespace API.DependecyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            var appUserMappingProfile = ApplicationModule.AppUserMappingProfile();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(appUserMappingProfile);
            });
            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }

        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            return services;
        }
    
}
}
