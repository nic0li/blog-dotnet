using Blog.Data;
using Blog.Entities;
using Blog.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repositories;

public class Repository<Entity> : IRepository<Entity>
    where Entity : BaseEntity
{
    protected readonly AppDbContext _dbContext;

    public Repository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Entity?> GetByIdAsync(long id)
    {
        return await _dbContext.Set<Entity>()
            .FirstOrDefaultAsync(entity => entity.Id == id);
    }

    public async Task<IEnumerable<Entity>> GetAllAsync()
    {
        return await _dbContext.Set<Entity>()
            .ToListAsync();
    }

    public async Task AddAsync(Entity entity)
    {
        await _dbContext.Set<Entity>()
            .AddAsync(entity);
    }

    public void Update(Entity entity)
    {
        _dbContext.Set<Entity>()
            .Update(entity);
    }

    public void Delete(Entity entity)
    {
        _dbContext.Set<Entity>()
            .Remove(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}