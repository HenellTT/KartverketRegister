using Microsoft.AspNetCore.Identity;

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
    }
}

