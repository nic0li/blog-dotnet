using Blog.DTOs.User;

namespace Blog.DTOs.Comment;

public record CommentViewResponse(
    long Id,
    string Content,
    UserViewResponse User,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);