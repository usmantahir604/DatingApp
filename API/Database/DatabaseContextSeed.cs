using API.Entities;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace API.Database
{
    public class DatabaseContextSeed
    {
        public static async Task SeedData(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            await AddRoles(roleManager);
            await AddUsers(userManager);
        }

        private static async Task AddRoles(RoleManager<ApplicationRole> roleManager)
        {
            var existingRoles = roleManager.Roles.ToList();
            var newRoles=new List<ApplicationRole>()
            {
                new ApplicationRole(){Name="Admin"},
                new ApplicationRole(){Name="Member"},
                new ApplicationRole(){Name="Moderator"}
            };

            var result = newRoles.Where(er => existingRoles.All(newR => !string.Equals(newR.Name, er.Name, StringComparison.CurrentCultureIgnoreCase)));
            foreach (var role in result)
            {
                await roleManager.CreateAsync(role);
            }
        }
        private static async Task AddUsers(UserManager<ApplicationUser> userManager)
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
                var created=await userManager.CreateAsync(user, "Test@123");
                if (created.Succeeded)
                  await  userManager.AddToRoleAsync(user, "Member");
            }

            var admin = new ApplicationUser
            {
                UserName = "admin"
            };
            var adminResult = existingUsers.FirstOrDefault(x=>x.UserName == admin.UserName);
            if(adminResult == null)
            {
                var created = await userManager.CreateAsync(admin, "Test@123");
                if (created.Succeeded)
                    await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
            }
        }

        
    }
}
