using Blog.Data;
using Blog.Entities;
using Blog.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repositories;

public class CategoryRepository(AppDbContext dbContext) : Repository<Category>(dbContext), ICategoryRepository
{
    public override async Task<Category?> GetByIdAsync(long id)
    {
        return await _dbContext.Categories
            .FirstOrDefaultAsync(category => category.Id == id);
    }

    public override async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _dbContext.Categories
            .ToListAsync();
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _dbContext.Categories
            .FirstOrDefaultAsync(category => category.Name == name);
    }

    public async Task<IEnumerable<Category>> GetAllByNameContainingAsync(string name)
    {
        return await _dbContext.Categories
            .Where(category => category.Name.Contains(name))
            .ToListAsync();
    }
}