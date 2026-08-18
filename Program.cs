
using Blog.Data;
using Blog.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Blog;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter());
            });

        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerConfiguration();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddRepositories();

        builder.Services.AddServices();

        builder.Services.AddSecurity(
            builder.Configuration);

        builder.Services.AddExceptionHandling();

        builder.Services.AddCorsPolicy();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseCors("CorsPolicy");

        app.UseExceptionHandling();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.UseSwaggerConfiguration();

        app.Run();
    }
}
