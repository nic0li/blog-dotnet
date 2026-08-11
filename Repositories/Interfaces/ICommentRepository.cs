using Blog.Entities;

namespace Blog.Repositories.Interfaces;

public interface ICommentRepository : IRepository<Comment>
{
    Task<IEnumerable<Comment>> GetAllByPostIdAsync(long postId);
}