using Blog.Enums;

namespace Blog.DTOs.User;

public record UserResponse(
    long Id,
    string Email,
    string? Name,
    string? Photo,
    string? Bio,
    UserRole Role
);