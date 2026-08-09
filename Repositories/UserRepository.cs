using Blog.Data;
using Blog.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(user => user.Email == email);
    }
}