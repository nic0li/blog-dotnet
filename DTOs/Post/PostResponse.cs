using Blog.DTOs.Category;
using Blog.DTOs.Comment;
using Blog.DTOs.User;
using System.Text.Json.Serialization;

namespace Blog.DTOs.Post;

public record PostResponse(
    long Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    CategoryResponse Category,
    UserProfileResponse User,

    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    List<CommentResponse>? Comments
);