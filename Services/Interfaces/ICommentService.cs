using Blog.DTOs.Comment;

namespace Blog.Services.Interfaces;

public interface ICommentService : ICrudService<
    CommentResponse,
    CommentViewResponse,
    CommentCreateRequest,
    CommentUpdateRequest>
{
    Task<IEnumerable<CommentViewResponse>> GetAllAsync();
}