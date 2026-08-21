using Blog.DTOs.Comment;

namespace Blog.Services.Interfaces;

public interface ICommentService : ICrudService<
    CommentResponse,
    CommentResponse,
    CommentCreateRequest>
{
    Task<CommentResponse> UpdateAsync(long id, CommentUpdateRequest request);

    Task<IEnumerable<CommentResponse>> GetAllAsync();
}