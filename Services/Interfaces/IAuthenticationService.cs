using Blog.DTOs.Authentication;

namespace Blog.Services.Interfaces;

public interface IAuthenticationService
{
    Task<LoginResponse> AuthenticateAsync(LoginRequest request);
}