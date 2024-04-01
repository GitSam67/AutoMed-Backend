using AutoMed_Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace AutoMed_Backend
{
    public class SuperAdminAssign
    {
        public static async Task CreateApplicationAdministrator(IServiceProvider serviceProvider)
        {
            try
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

                IdentityResult result;

                var isRoleExist = await roleManager.RoleExistsAsync("SuperAdmin");

                if (!isRoleExist)
                {
                    result = await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                }

                var user = await userManager.FindByEmailAsync("admin@gmail.com");

                if (user == null)
                {
                    var defaultUser = new IdentityUser()
                    {
                        UserName = "AdminBhai",
                        Email = "admin@gmail.com"
                    };

                    var regUser = await userManager.CreateAsync(defaultUser, "Admin@67");
                    await userManager.AddToRoleAsync(defaultUser, "SuperAdmin");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
