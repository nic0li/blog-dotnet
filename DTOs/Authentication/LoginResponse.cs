using Blog.DTOs.User;

namespace Blog.DTOs.Authentication;

public record LoginResponse(
    UserResponse User,
    string Token
);