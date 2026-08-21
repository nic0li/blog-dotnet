using Blog.Data;
using Blog.Entities;
using Blog.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repositories;

public class PostRepository(AppDbContext dbContext) : Repository<Post>(dbContext), IPostRepository
{
    public override async Task<Post?> GetByIdAsync(long id)
    {
        return await _dbContext.Posts
            .Include(post => post.User)
            .Include(post => post.Category)
            .Include(post => post.Comments)
            .FirstOrDefaultAsync(post => post.Id == id);
    }

    public override async Task<IEnumerable<Post>> GetAllAsync()
    {
        return await _dbContext.Posts
            .Include(post => post.User)
            .Include(post => post.Category)
            .Include(post => post.Comments)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetAllByUserIdAsync(long userId)
    {
        return await _dbContext.Posts
            .Include(post => post.User)
            .Include(post => post.Category)
            .Include(post => post.Comments)
            .Where(post => post.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetAllByTitleAsync(string title)
    {
        return await _dbContext.Posts
            .Include(post => post.User)
            .Include(post => post.Category)
            .Include(post => post.Comments)
            .Where(post => post.Title.Contains(title))
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetAllByCategoryNameAsync(string category)
    {
        return await _dbContext.Posts
            .Include(post => post.User)
            .Include(post => post.Category)
            .Include(post => post.Comments)
            .Where(post => post.Category.Name.Contains(category))
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetAllByTitleAndCategoryNameAsync(string title, string category)
    {
        return await _dbContext.Posts
            .Include(post => post.User)
            .Include(post => post.Category)
            .Include(post => post.Comments)
            .Where(post =>
                post.Title.Contains(title) &&
                post.Category.Name.Contains(category))
            .ToListAsync();
    }
}