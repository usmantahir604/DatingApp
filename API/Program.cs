using API.Database;
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
                        if (context.Database.IsSqlServer())
                        {
                            await context.Database.MigrateAsync();
                        }

                        //await DatabaseContextSeed.SeedDataAsync(context);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex}");
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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}


// EF Core Migration commands:
// get - help
// get-help Add-Migration
// Add-Migration InitialCreate