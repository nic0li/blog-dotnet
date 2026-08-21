using Blog.Entities;
using Blog.Exceptions;
using Blog.Repositories.Interfaces;

namespace Blog.Services
{
    public abstract class EntityService<Entity>(IRepository<Entity> repository)
        where Entity : BaseEntity
    {
        protected readonly IRepository<Entity> Repository = repository;

        public async Task<Entity> GetEntityByIdAsync(long id)
        {
            var entity = await Repository.GetByIdAsync(id);

            return entity is null
                ? throw new NotFoundException(
                    $"{typeof(Entity).Name} not found")
                : entity;
        }
    }
}