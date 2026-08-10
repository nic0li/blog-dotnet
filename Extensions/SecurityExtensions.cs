using Blog.Repositories;
using Blog.Security;
using Blog.Security.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
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

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var repository =
                            context.HttpContext.RequestServices
                                .GetRequiredService<IUserRepository>();

                        var userIdClaim =
                            context.Principal?
                                .FindFirst(ClaimTypes.NameIdentifier);

                        if (userIdClaim is null ||
                            !long.TryParse(
                                userIdClaim.Value,
                                out var userId))
                        {
                            context.Fail("Invalid user identity.");
                            return;
                        }

                        var user =
                            await repository.GetByIdAsync(userId);

                        if (user is null)
                        {
                            context.Fail("User not found.");
                        }
                    }
                };
            });

        services.AddAuthorization();

        services.AddHttpContextAccessor();

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}