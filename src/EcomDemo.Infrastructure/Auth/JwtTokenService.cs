using EcomDemo.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcomDemo.Infrastructure.Auth;

public sealed class JwtTokenService(IConfiguration config) : ITokenService
{
    public string IssueToken(string subject, IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            config["Jwt:Key"] ?? "ecom-demo-public-secret-32b-please-rotate"));

        var token = new JwtSecurityToken(
            issuer: "ecom-demo",
            audience: "ecom-demo",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}