using Blog.DTOs.Authentication;
using Blog.Entities;

namespace Blog.Services.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);

    Task<User> GetAuthenticatedUserAsync();
}