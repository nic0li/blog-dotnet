using Blog.Security;
using Blog.Security.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Blog.Extensions;

public static class SecurityExtensions
{
    public static IServiceCollection AddSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSecret = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException(
                "JWT secret is not configured.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtSecret)),

                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
            });

        services.AddAuthorization();

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordService, PasswordService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}