using Blog.Entities;

namespace Blog.Repositories.Interfaces;

public interface IRepository<Entity>
    where Entity : BaseEntity
{
    Task<Entity?> GetByIdAsync(long id);

    Task<IEnumerable<Entity>> GetAllAsync();

    Task AddAsync(Entity entity);

    void Update(Entity entity);

    void Delete(Entity entity);

    Task SaveChangesAsync();
}