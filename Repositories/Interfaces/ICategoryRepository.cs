using Blog.Entities;
using Blog.Repositories.Interfaces;

namespace Blog.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);

    Task<IEnumerable<Category>> GetByNameContainingAsync(string name);
}