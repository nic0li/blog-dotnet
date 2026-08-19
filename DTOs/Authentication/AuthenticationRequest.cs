namespace Blog.DTOs.Authentication;

public record AuthenticationRequest(
    string Login,
    string Password
);