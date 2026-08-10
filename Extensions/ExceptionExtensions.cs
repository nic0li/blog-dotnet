using Blog.Exceptions;

namespace Blog.Extensions;

public static class ExceptionExtensions
{
    public static IServiceCollection AddExceptionHandling(
        this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }

    public static WebApplication UseExceptionHandling(
        this WebApplication app)
    {
        app.UseExceptionHandler();

        return app;
    }
}