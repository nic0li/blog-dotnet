namespace Blog.DTOs.Post;

public record PostUpdateRequest(
    string? Title,
    string? Content,
    long? CategoryId
);