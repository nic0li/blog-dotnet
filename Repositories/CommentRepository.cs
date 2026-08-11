using Blog.Data;
using Blog.Entities;
using Blog.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repositories;

public class CommentRepository(AppDbContext dbContext) : Repository<Comment>(dbContext), ICommentRepository
{
    public override async Task<Comment?> GetByIdAsync(long id)
    {
        return await _dbContext.Comments
            .Include(comment => comment.User)
            .FirstOrDefaultAsync(comment => comment.Id == id);
    }

    public override async Task<IEnumerable<Comment>> GetAllAsync()
    {
        return await _dbContext.Comments
            .Include(comment => comment.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetAllByPostIdAsync(long postId)
    {
        return await _dbContext.Comments
            .Include(comment => comment.User)
            .Where(comment => comment.PostId == postId)
            .ToListAsync();
    }
}