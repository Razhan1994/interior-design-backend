using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InteriorMarketplace.Modules.Identity;
using Microsoft.IdentityModel.Tokens;

namespace InteriorMarketplace.WebApi;

internal sealed class JwtDevelopmentTokenService(string jwtKey)
{
    public const string Issuer = "InteriorMarketplace";
    public const string Audience = "InteriorMarketplace";

    public object Create(string role)
    {
        var userId = role.Equals("Vendor", StringComparison.OrdinalIgnoreCase)
            ? SeedUsers.VendorId
            : SeedUsers.HomeownerId;
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var token = new JwtSecurityToken(
            Issuer,
            Audience,
            claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256));

        return new
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            UserId = userId
        };
    }
}
