using Blog.Entities;
using Blog.Repositories.Interfaces;

namespace Blog.Repositories;

public interface IPostRepository : IRepository<Post>
{
    Task<IEnumerable<Post>> GetByUserIdAsync(long userId);

    Task<IEnumerable<Post>> GetByTitleContainingAsync(string title);

    Task<IEnumerable<Post>> GetByCategoryContainingAsync(string category);

    Task<IEnumerable<Post>> GetByTitleContainingAndCategoryContainingAsync(
        string title,
        string category);
}