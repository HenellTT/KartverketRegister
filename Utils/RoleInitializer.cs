using KartverketRegister.Auth;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace KartverketRegister.Utilities
{
    public static class RoleInitializer
    {
        public static async Task SeedRoles(RoleManager<IdentityRole<int>> roleManager)
        {
            string[] roles = { "Admin", "Pilot", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int> { Name = role });
                }
            }
        }
    

    public static async Task SeedDefaultUsers(UserManager<AppUser> userManager)
        {
            var users = new[]
   {
        new {
            Email = "admin@gmail.com",
            Password = "Admin123!",
            Role = "Admin",
            FirstName = "Admin",
            LastName = "User",
            Organization = "Admin Org"
        },
        new {
            Email = "pilot@gmail.com",
            Password = "Pilot123!",
            Role = "Pilot",
            FirstName = "Pilot",
            LastName = "User",
            Organization = "Pilot Org"
        }
    };
            foreach (var u in users)
            {
                var existing = await userManager.FindByEmailAsync(u.Email);
                if (existing == null)
                {
                    var user = new AppUser
                    {
                        UserName = u.Email,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Organization = u.Organization,
                        UserType = u.Role,
                        CreatedAt = DateTime.UtcNow
                    };

                    var result = await userManager.CreateAsync(user, u.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, u.Role);
                    }
                }
            }
        }

    }
}


