using Blog.Entities;

namespace Blog.Repositories.Interfaces;

public interface IPostRepository : IRepository<Post>
{
    Task<IEnumerable<Post>> GetAllByUserIdAsync(long userId);

    Task<IEnumerable<Post>> GetAllByTitleContainingAsync(string title);

    Task<IEnumerable<Post>> GetAllByCategoryNameContainingAsync(string category);

    Task<IEnumerable<Post>> GetAllByTitleContainingAndCategoryNameContainingAsync(
        string title,
        string category);
}