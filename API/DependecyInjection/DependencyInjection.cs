using API.CloudinaryServices;
using API.Common.Models;
using API.DAL.Like;
using API.DAL.Message;
using API.DAL.User;
using API.DAL.User.Models;
using API.Database;
using API.Entities;
using API.Helpers;
using API.Identity;
using API.Interfaces;
using API.Modules;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.DependecyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            var applicationUserMappingProfile = ApplicationModule.ApplicationUserMappingProfile();
            var messageMappingProfile = ApplicationModule.MessageMappingProfile();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(applicationUserMappingProfile);
                mc.AddProfile(messageMappingProfile);
            });
            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }
        public static IServiceCollection AddDBServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName));
            });
            //services.Configure<IdentityOptions>(options =>
            //{
            //    options.Password.RequiredLength = 6;
            //});
            //services.AddIdentityCore<ApplicationUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<DatabaseContext>()
            //    .AddDefaultTokenProviders();
            return services;
        }

        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CloudinarySettings>(configuration.GetSection(nameof(CloudinarySettings)));
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IPhotoService, PhotoService>();
            services.AddTransient<ILikeService, LikeService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<LogUserActivity>();
            return services;
        }

        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddRoles<ApplicationRole>()
                .AddRoleManager<RoleManager<ApplicationRole>>()
                .AddSignInManager<SignInManager<ApplicationUser>>()
                .AddRoleValidator<RoleValidator<ApplicationRole>>()
                .AddEntityFrameworkStores<DatabaseContext>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<ITokenGenerator, TokenGenerator>();
            InitializeIdentityParameters(services, configuration);
            return services;
        }

        public static IServiceCollection AddFluentValidatationService(this IServiceCollection services)
        {
            //services.AddTransient<IValidator<CreateUserModel>, CreateUserModelValidator>();
            //services.AddTransient<IValidator<LoginUserModel>, LoginUserModelValidator>();
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
                //Authenticating to SignalR
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
            });

        }

    }
}
