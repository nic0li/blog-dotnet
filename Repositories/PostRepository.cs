using Blog.Data;
using Blog.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repositories;

public class PostRepository : Repository<Post>, IPostRepository
{
    public PostRepository(AppDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<IEnumerable<Post>> GetByUserIdAsync(long userId)
    {
        return await _dbContext.Posts
            .Where(post => post.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetByTitleContainingAsync(
        string title)
    {
        return await _dbContext.Posts
            .Where(post => post.Title.Contains(title))
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetByCategoryContainingAsync(
        string category)
    {
        return await _dbContext.Posts
            .Where(post => post.Category.Name.Contains(category))
            .ToListAsync();
    }

    public async Task<IEnumerable<Post>> GetByTitleContainingAndCategoryContainingAsync(
        string title,
        string category)
    {
        return await _dbContext.Posts
            .Where(post =>
                post.Title.Contains(title) &&
                post.Category.Name.Contains(category))
            .ToListAsync();
    }
}