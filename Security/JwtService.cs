using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blog.Security.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Blog.Security;

public class JwtService : IJwtService
{
    private readonly string _secret;
    private readonly long _expiration;

    public JwtService(IConfiguration configuration)
    {
        _secret = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException(
                "JWT secret is not configured.");

        _expiration = long.Parse(
            configuration["Jwt:Expiration"]
                ?? throw new InvalidOperationException(
                    "JWT expiration is not configured."));
    }

    public string GenerateToken(long userId)
    {
        var now = DateTime.UtcNow;

        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                userId.ToString()),
            new Claim(
                JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(now)
                    .ToUnixTimeSeconds()
                    .ToString(),
                ClaimValueTypes.Integer64)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_secret));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: now.AddMilliseconds(_expiration),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    public long ExtractUserId(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        var jwtToken = handler.ReadJwtToken(token);

        var subject = jwtToken
            .Claims
            .First(claim => claim.Type == JwtRegisteredClaimNames.Sub)
            .Value;

        return long.Parse(subject);
    }
}