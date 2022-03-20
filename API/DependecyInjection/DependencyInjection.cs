using API.Common.Models;
using API.DAL.User;
using API.DAL.User.Models;
using API.Identity;
using API.Interfaces;
using API.Modules;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<ITokenGenerator, TokenGenerator>();
            InitializeIdentityParameters(services, configuration);
            return services;
        }

        public static IServiceCollection AddFluentValidatationService(this IServiceCollection services)
        {
            services.AddTransient<IValidator<CreateUserModel>, CreateUserModelValidator>();
            services.AddTransient<IValidator<LoginUserModel>, LoginUserModelValidator>();
            return services;
        }

        private static void InitializeIdentityParameters(IServiceCollection services, IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection(nameof(JwtSettings));
            services.Configure<JwtSettings>(appSettingsSection);
            //// configure jwt authentication
            var jwtSettings = appSettingsSection.Get<JwtSettings>();

            var tokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = tokenValidationParameters;
            });
        }

    }
}
