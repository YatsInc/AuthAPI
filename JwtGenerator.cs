using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AuthAPI
{
    public class JwtGenerator
    {
        readonly RsaSecurityKey _key;
        public JwtGenerator(string signingKey)
        {
            RSA privateRSA = RSA.Create();
            privateRSA.ImportRSAPrivateKey(Convert.FromBase64String(signingKey), out _);
            _key = new RsaSecurityKey(privateRSA);
        }

        public string CreateUserAuthToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = "myApi",
                Issuer = "AuthService",
                Subject = new ClaimsIdentity(new Claim[]
                {
                            new Claim(ClaimTypes.Sid, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.RsaSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
