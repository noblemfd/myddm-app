using DDM.API.Infrastructure.Data.Identiity;
using DDM.API.Infrastructure.Entities.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Infrastructure.Data.Application
{
    public static class DDMDbInitializer
    {
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }
        public static void SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                ApplicationRole role = new ApplicationRole();
                role.Name = "Admin";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Merchant").Result)
            {
                ApplicationRole role = new ApplicationRole();
                role.Name = "Merchant";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Staff").Result)
            {
                ApplicationRole role = new ApplicationRole();
                role.Name = "Staff";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Customer").Result)
            {
                ApplicationRole role = new ApplicationRole();
                role.Name = "Customer";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
        }
        public static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByNameAsync("admin").Result == null)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = "admin",
                    Email = "admin@africa.int.zenithbank.com",
                    NormalizedEmail = "ADMIN@AFRICA.INT.ZENITHBANK.COM",
                    FirstName = "AdminFN",
                    LastName = "AdminLN",
                    NormalizedUserName = "ADMIN",
                    MobileNumber = "1234567890",
                    PhoneNumber = "+1234567890"
                };
                IdentityResult result = userManager.CreateAsync
                (user, "Admin*123").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }

            if (userManager.FindByNameAsync("trdev8").Result == null)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = "trdev8",
                    Email = "trdev8@africa.int.zenithbank.com",
                    NormalizedEmail = "TRDEV8@AFRICA.INT.ZENITHBANK.COM",
                    FirstName = "trdev8FN",
                    LastName = "trdev8LN",
                    NormalizedUserName = "TRDEV8",
                    MobileNumber = "08011221122",
                    PhoneNumber = "+08011221122"
                };
                IdentityResult result = userManager.CreateAsync
                (user, "Admin*123").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }
    }
}
