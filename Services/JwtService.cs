using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace atmglobalapi.Services
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string email, string role);
        Dictionary<string, string> DecodeToken(string token);
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(string userId, string email, string role)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim("role", role), // ✅ custom role also added
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var authSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _config["JWT:ValidIssuer"],
                audience: _config["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(5),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Dictionary<string, string> DecodeToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var claims = new Dictionary<string, string>();

            foreach (var claim in jwt.Claims)
                claims[claim.Type] = claim.Value;

            return claims;
        }
    }
}
