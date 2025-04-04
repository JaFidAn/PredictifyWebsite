using Application.Utulity;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Persistence.Contexts.Data;

public class DbInitializer
{
    public static async Task SeedData(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Seed Roles
        if (!await roleManager.RoleExistsAsync(SD.Role_Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
        }

        if (!await roleManager.RoleExistsAsync(SD.Role_User))
        {
            await roleManager.CreateAsync(new IdentityRole(SD.Role_User));
        }

        // Seed Admin User
        if (!userManager.Users.Any())
        {
            var adminUser = new AppUser
            {
                FullName = "Rasim Alagezov",
                UserName = "rasim",
                Email = "r.alagezov@gmail.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, SD.Admin_Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, SD.Role_Admin);
            }
        }


        await context.SaveChangesAsync();
    }
}

