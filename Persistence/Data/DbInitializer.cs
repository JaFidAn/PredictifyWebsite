using Application.Utilities;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Persistence.Contexts.Data;

public class DbInitializer
{
    public static async Task SeedData(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // ✅ Seed Roles
        if (!await roleManager.RoleExistsAsync(SD.Role_Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
        }

        if (!await roleManager.RoleExistsAsync(SD.Role_User))
        {
            await roleManager.CreateAsync(new IdentityRole(SD.Role_User));
        }

        // ✅ Seed Admin User
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

        // ✅ Seed Outcomes
        if (!context.Outcomes.Any())
        {
            var outcomes = new List<Outcome>
            {
                new Outcome { Name = "Win", Code = SD.WIN, Description = "Team wins the match" },
                new Outcome { Name = "Draw", Code = SD.DRAW, Description = "Match ends in a draw" },
                new Outcome { Name = "Lose", Code = SD.LOSE, Description = "Team loses the match" },
                new Outcome { Name = "Over 3.5 Goals", Code = SD.OVER_3_5, Description = "Total goals > 3.5" },
                new Outcome { Name = "Under 1.5 Goals", Code = SD.UNDER_1_5, Description = "Total goals ≤ 1.5" }
            };

            context.Outcomes.AddRange(outcomes);
        }

        await context.SaveChangesAsync();
    }
}
