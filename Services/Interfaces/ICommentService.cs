using Blog.DTOs.Comment;

namespace Blog.Services.Interfaces;

public interface ICommentService : ICrudService<
    CommentResponse,
    CommentViewResponse,
    CommentCreateRequest>
{
    Task<CommentResponse> UpdateAsync(long id, CommentUpdateRequest request);

    Task<IEnumerable<CommentViewResponse>> GetAllAsync();
}