using Blog.DTOs.Post;
using Blog.DTOs.User;
using System.Text.Json.Serialization;

namespace Blog.DTOs.Comment;

public record CommentResponse(
    long Id,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    UserProfileResponse User,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    PostResponse? Post
);