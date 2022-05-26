using API.Database;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var context = services.GetRequiredService<DatabaseContext>();
                        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
                        if (context.Database.IsSqlServer())
                        {
                            await context.Database.MigrateAsync();
                        }

                        await DatabaseContextSeed.SeedData(userManager,roleManager);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex}");
                        throw ex;
                    }
                }
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Host terminated unexpectedly {ex}");
            }
            finally
            {
                Console.WriteLine($"finally");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureLogging((ctx, builder) =>
                        {
                            builder.AddConfiguration(ctx.Configuration.GetSection("Logging"));
                            builder.AddFile(o => o.RootPath = ctx.HostingEnvironment.ContentRootPath);
                        }).UseStartup<Startup>();
                });
    }
}


// EF Core Migration commands:
// get - help
// get-help Add-Migration
// Add-Migration InitialCreate