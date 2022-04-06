using API.Entities;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace API.Database
{
    public class DatabaseContextSeed
    {
        public static async Task SeedUsers(UserManager<ApplicationUser> userManager)
        {
            var existingUsers = userManager.Users.ToList();
            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<ApplicationUser>>(userData);
            if (users == null) return;
            var newUsers = users.Where(eu => existingUsers.All(u => !string.Equals(u.UserName, eu.UserName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            newUsers.ForEach(u =>
            {
                u.UserName = u.UserName.ToLower();
            });
            foreach (var user in newUsers)
            {
                await userManager.CreateAsync(user, "Test@123");
            }
        }
    }
}
