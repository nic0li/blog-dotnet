namespace Blog.Services.Interfaces
{
    public interface IEntityService<Entity>
    {
        Task<Entity> GetEntityByIdAsync(long id);
    }
}
