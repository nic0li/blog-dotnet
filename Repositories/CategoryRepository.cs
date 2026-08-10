using Blog.Data;
using Blog.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _dbContext.Categories
            .FirstOrDefaultAsync(category => category.Name == name);
    }

    public async Task<IEnumerable<Category>> GetAllByNameContainingAsync(
        string name)
    {
        return await _dbContext.Categories
            .Where(category => category.Name.Contains(name))
            .ToListAsync();
    }
}