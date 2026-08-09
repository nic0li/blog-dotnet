using Blog.Entities;
using Blog.Repositories.Interfaces;

namespace Blog.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}