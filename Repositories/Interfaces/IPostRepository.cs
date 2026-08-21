using Blog.Entities;

namespace Blog.Repositories.Interfaces;

public interface IPostRepository : IRepository<Post>
{
    Task<IEnumerable<Post>> GetAllByUserIdAsync(long userId);

    Task<IEnumerable<Post>> GetAllByTitleAsync(string title);

    Task<IEnumerable<Post>> GetAllByCategoryNameAsync(string category);

    Task<IEnumerable<Post>> GetAllByTitleAndCategoryNameAsync(string title, string category);
}