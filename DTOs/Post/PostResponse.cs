using Blog.DTOs.Category;
using Blog.DTOs.User;

namespace Blog.DTOs.Post;

public record PostResponse(
    long Id,
    string Title,
    string Content,
    CategoryResponse Category,
    UserResponse User,
    DateTime CreatedAt,
    DateTime UpdatedAt
);