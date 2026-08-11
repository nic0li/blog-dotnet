using Blog.Entities;

namespace Blog.Repositories.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);

    Task<IEnumerable<Category>> GetAllByNameContainingAsync(string name);
}