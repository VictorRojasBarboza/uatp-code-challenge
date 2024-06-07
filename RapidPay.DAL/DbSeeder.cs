using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RapidPay.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidPay.DAL
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using (var context = new ContextDB(
                serviceProvider.GetRequiredService<DbContextOptions<ContextDB>>()))
            {
                await context.Database.EnsureCreatedAsync();

                var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                await SeedRolesAsync(roleManager);
                await SeedUsersAsync(userManager);
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
        }

        private static async Task SeedUsersAsync(UserManager<User> userManager)
        {
            if (await userManager.FindByNameAsync("testuser") == null)
            {
                var user = new User
                {
                    UserName = "testuser",
                    Email = "testuser@example.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Password123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }

            if (await userManager.FindByNameAsync("admin") == null)
            {
                var admin = new User
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin@1234");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
