using Blog.Entities;
using Blog.Repositories.Interfaces;

namespace Blog.Repositories;

public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetByPostIdAsync(long postId);
}