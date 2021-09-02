﻿using AuthAPI.Constants;
using AuthAPI.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace AuthAPI.Data
{
    public class ApplicationDbContextSeed
    {
        public static async Task SeedEssentialsAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Authorization.Roles.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Authorization.Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Authorization.Roles.PremiumUser.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Authorization.Roles.NonPremiumUser.ToString()));
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = Authorization.DefaultUserName,
                Email = Authorization.DefaultEmail,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                await userManager.CreateAsync(defaultUser, Authorization.DefaultPassword);
                await userManager.AddToRoleAsync(defaultUser, Authorization.DefaultRole.ToString());
            }
        }
    }
}
