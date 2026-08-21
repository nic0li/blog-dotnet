namespace Blog.DTOs.User;

public record UserProfileResponse(
    long Id,
    string? Name,
    string? Photo,
    string? Bio
);