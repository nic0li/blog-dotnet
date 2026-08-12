using Blog.DTOs.Category;
using Blog.DTOs.Comment;
using Blog.DTOs.User;

namespace Blog.DTOs.Post;

public record PostViewResponse(
    long Id,
    string Title,
    string Content,
    CategoryResponse Category,
    UserViewResponse User,
    IEnumerable<CommentViewResponse> Comments,
    DateTime CreatedAt,
    DateTime UpdatedAt
);