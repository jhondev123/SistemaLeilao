using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SistemaLeilao.Core.Application.Interfaces;
using SistemaLeilao.Infrastructure.Indentity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SistemaLeilao.Infrastructure.Services.JwtToken
{
    public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGeneratorService
    {
        public string GenerateToken(Guid userExternalId, string email, string userName, IList<string> roles)
        {
            var secret = configuration["JwtSettings:Secret"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userExternalId.ToString()),
                new(JwtRegisteredClaimNames.Email, email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Name, userName),
                new(ClaimTypes.NameIdentifier, userExternalId.ToString()),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
