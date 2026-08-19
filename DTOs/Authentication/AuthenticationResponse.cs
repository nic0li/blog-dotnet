using Blog.DTOs.User;

namespace Blog.DTOs.Authentication;

public record AuthenticationResponse(
    UserResponse User,
    string Token
);