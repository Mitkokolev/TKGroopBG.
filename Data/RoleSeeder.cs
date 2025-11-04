using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace TKGroopBG.Data
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var roleName = "Admin";
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var adminEmail = "admin@tkgroopbg.com";
            var adminPassword = "Admin!123";

            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true 
                };

                var createResult = await userManager.CreateAsync(admin, adminPassword);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, roleName);
                }
                else
                {
                    
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(admin, roleName))
                {
                    await userManager.AddToRoleAsync(admin, roleName);
                }
            }
        }
    }
}
