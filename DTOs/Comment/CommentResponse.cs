using Blog.DTOs.User;

namespace Blog.DTOs.Comment;

public record CommentResponse(
    long Id,
    string Content,
    UserResponse User,
    DateTime CreatedAt,
    DateTime UpdatedAt
);