namespace Blog.DTOs.User;

public record UserViewResponse(
    long Id,
    string? Name,
    string? Photo,
    string? Bio
);