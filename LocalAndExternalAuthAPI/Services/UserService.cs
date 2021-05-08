using LocalAndExternalAuthAPI.Constants;
using LocalAndExternalAuthAPI.IServices;
using LocalAndExternalAuthAPI.Models;
using LocalAndExternalAuthAPI.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace LocalAndExternalAuthAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly JWT jwt;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.jwt = jwt.Value;
        }

        public async Task<string> RegisterAsync(RegisterModel model)
        {
            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Username,
                Email = model.Email
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, Authorization.DefaultRole.ToString());
            }
            else
                foreach (var error in result.Errors)
                    return error.Description; // Simplified variant of ModelState errors

            return $"User {user.UserName} was successfully registered";
        }
    }
}
