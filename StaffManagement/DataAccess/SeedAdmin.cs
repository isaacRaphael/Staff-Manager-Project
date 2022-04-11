using Microsoft.AspNetCore.Identity;
using StaffManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManagement.DataAccess
{
    public static class SeedAdmin
    {
        public async static Task Seed(AppDbContext dbContext,
            RoleManager<IdentityRole> roleManager,
            UserManager<Staff> userManager)
        {
            var MainRole = "Admin";
            dbContext.Database.EnsureCreated();

            if (!roleManager.Roles.Any())
                await roleManager.CreateAsync(new IdentityRole(MainRole));

            var user = await userManager.FindByEmailAsync("chukwuemeka@thebulb.com");
            if (user is null)
            {
                var adminStaff = new Staff
                {
                    Email = "chukwuemeka@thebulb.com",
                    UserName = "jhay1",
                    FirstName = "Chukwuemeka",
                    LastName = "Okereke",
                    PhotoPath = ""
                };
                await userManager.CreateAsync(adminStaff, "1234567890abc");
                await userManager.AddToRoleAsync(adminStaff, MainRole);

            }
        }
    }
}
