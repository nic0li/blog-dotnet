using Blog.Data;
using Blog.Entities;
using Blog.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repositories;

public class UserRepository(AppDbContext dbContext) : Repository<User>(dbContext), IUserRepository
{
    public override async Task<User?> GetByIdAsync(long id)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == id);
    }

    public override async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dbContext.Users
            .ToListAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(user => user.Email == email);
    }
}