using Microsoft.OpenApi;

namespace Blog.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiConfiguration(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Blog",
                    Version = "1.0"
                });

            options.TagActionsBy(api =>
            [
                api.ActionDescriptor.RouteValues["controller"] switch
                {
                    "Category" => "Categories",
                    "Comment" => "Comments",
                    "Post" => "Posts",
                    "User" => "Users",
                    "Authentication" => "Authentication",
                    _ => api.ActionDescriptor.RouteValues["controller"]!
                }
            ]);

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token."
                });

            options.AddSecurityRequirement(
                document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] =
                        []
                });
        });

        return services;
    }

    public static WebApplication UseSwaggerConfiguration(
        this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}