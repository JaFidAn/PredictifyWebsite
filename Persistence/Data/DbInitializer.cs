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
                new Outcome { Name = "Win", Code = "WIN", Description = "Team wins the match" },
                new Outcome { Name = "Draw", Code = "DRAW", Description = "Match ends in a draw" },
                new Outcome { Name = "Lose", Code = "LOSE", Description = "Team loses the match" },
                new Outcome { Name = "Over 2.5 Goals", Code = "OVER_2_5", Description = "Total goals in the match is over 2.5" },
                new Outcome { Name = "Under 2.5 Goals", Code = "UNDER_2_5", Description = "Total goals in the match is under 2.5" },
                new Outcome { Name = "Both Teams To Score (BTTS)", Code = "BTTS", Description = "Both teams scored at least 1 goal" },
                new Outcome { Name = "Both Teams Not To Score (BTNS)", Code = "BTNS", Description = "At least one team did not score" },
                new Outcome { Name = "Team1 Scores Over 1.5 Goals", Code = "T1_OVER_1_5", Description = "Team1 scored more than 1.5 goals" },
                new Outcome { Name = "Team2 Scores Over 1.5 Goals", Code = "T2_OVER_1_5", Description = "Team2 scored more than 1.5 goals" }
            };

            context.Outcomes.AddRange(outcomes);
        }

        await context.SaveChangesAsync();
    }
}
