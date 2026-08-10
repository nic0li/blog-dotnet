namespace Blog.DTOs.Authentication;

public record LoginRequest(
    string Login,
    string Password
);