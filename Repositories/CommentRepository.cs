using Blog.Data;
using Blog.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Repositories;

public class CommentRepository
    : Repository<Comment>, ICommentRepository
{
    public CommentRepository(AppDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<IEnumerable<Comment>> GetByPostIdAsync(long postId)
    {
        return await _dbContext.Comments
            .Where(comment => comment.PostId == postId)
            .ToListAsync();
    }
}