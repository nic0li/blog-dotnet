using Blog.DTOs.Comment;
using Blog.Entities;

namespace Blog.Services.Interfaces;

public interface ICommentService : IEntityService<Comment>
{
    Task<CommentResponse> CreateAsync(long postId, CommentRequest request);

    Task<CommentResponse> UpdateAsync(long id, CommentRequest request);

    Task DeleteAsync(long id);

    Task<CommentResponse> GetByIdAsync(long id);

    Task<IEnumerable<CommentResponse>> GetAllAsync();
}