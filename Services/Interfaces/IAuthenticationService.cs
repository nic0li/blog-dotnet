using Blog.DTOs.Authentication;
using Blog.Entities;

namespace Blog.Services.Interfaces;

public interface IAuthenticationService
{
    Task<LoginResponse> AuthenticateAsync(LoginRequest request);

    Task<User> GetAuthenticatedUserAsync();
}