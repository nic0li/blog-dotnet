using Blog.Repositories;
using Blog.Repositories.Interfaces;

namespace Blog.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();

        return services;
    }
}