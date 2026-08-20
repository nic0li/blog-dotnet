using Blog.Entities;
using Blog.Exceptions;
using Blog.Repositories.Interfaces;
using Blog.Services.Interfaces;

public abstract class CrudService<
    Entity,
    Response,
    ViewResponse,
    CreateRequest,
    UpdateRequest>
    : ICrudService<
        Response,
        ViewResponse,
        CreateRequest>
    where Entity : BaseEntity
{
    protected readonly IRepository<Entity> Repository;

    protected CrudService(IRepository<Entity> repository)
    {
        Repository = repository;
    }

    public async Task<Entity> GetEntityByIdAsync(long id)
    {
        var entity = await Repository.GetByIdAsync(id);

        if (entity is null)
        {
            throw new NotFoundException(
                $"{typeof(Entity).Name} not found");
        }

        return entity;
    }

    public abstract Task<Response> CreateAsync(CreateRequest request);

    public abstract Task DeleteAsync(long id);

    public abstract Task<ViewResponse> GetByIdAsync(long id);
}