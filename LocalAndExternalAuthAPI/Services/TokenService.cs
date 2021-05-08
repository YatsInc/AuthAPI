using LocalAndExternalAuthAPI.IServices;
using LocalAndExternalAuthAPI.Models;
using LocalAndExternalAuthAPI.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LocalAndExternalAuthAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly JWT jwt;

        public TokenService(UserManager<ApplicationUser> userManager, IOptions<JWT> jwt)
        {
            this.userManager = userManager;
            this.jwt = jwt.Value;
        }

        public async Task<AuthenticationModel> GetTokenAsync(TokenRequestModel model)
        {
            var authenticationModel = new AuthenticationModel();
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = $"No accounts registered with {model.Email}";
                return authenticationModel;
            }

            if(await userManager.CheckPasswordAsync(user, model.Password))
            {
                authenticationModel.Message = $"Hello {user.UserName}";
                authenticationModel.IsAuthenticated = true;
                JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);
                authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                authenticationModel.Email = user.Email;
                authenticationModel.UserName = user.UserName;
                var rolesList = await userManager.GetRolesAsync(user).ConfigureAwait(false);
                authenticationModel.Roles = rolesList.ToList();
                return authenticationModel;
            }

            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = $"Incorrect credentials for user {user.Email}";
            return authenticationModel;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var signinCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var claims = await GetClaimsIdentityAsync(user);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: jwt.Issuer,
                audience: jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwt.DurationInMinutes),
                signingCredentials: signinCredentials);

            return jwtSecurityToken;
        }

        private async Task<IEnumerable<Claim>> GetClaimsIdentityAsync(ApplicationUser user)
        {
            var userClaims = userManager.GetClaimsAsync(user).Result;
            var roles = await userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            }
            .Union(userClaims)
            .Union(roleClaims);

            return claims;
        }
    }
}
