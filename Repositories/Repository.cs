using Blog.Data;
using Blog.Entities;
using Blog.Repositories.Interfaces;

namespace Blog.Repositories;

public abstract class Repository<Entity>(AppDbContext dbContext) : IRepository<Entity>
    where Entity : BaseEntity
{
    protected readonly AppDbContext _dbContext = dbContext;

    public abstract Task<Entity?> GetByIdAsync(long id);

    public abstract Task<IEnumerable<Entity>> GetAllAsync();

    public async Task AddAsync(Entity entity)
    {
        await _dbContext.Set<Entity>().AddAsync(entity);
    }

    public void Update(Entity entity)
    {
        _dbContext.Set<Entity>().Update(entity);
    }

    public void Delete(Entity entity)
    {
        _dbContext.Set<Entity>().Remove(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}