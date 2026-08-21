using Blog.DTOs.Authentication;
using Blog.Entities;
using Blog.Mappers;
using Blog.Repositories.Interfaces;
using Blog.Security.Interfaces;
using Blog.Services.Interfaces;

namespace Blog.Services;

public class AuthenticationService(
    IUserRepository repository,
    IJwtService jwtService,
    IPasswordService passwordService,
    ICurrentUserService currentUserService) : IAuthenticationService
{
    private readonly IUserRepository _repository = repository;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<AuthenticationResponse> AuthenticateAsync(
        AuthenticationRequest request)
    {
        var user = await _repository.GetByEmailAsync(request.Login);

        if (user is null || !_passwordService.Verify(request.Password, user.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var token = _jwtService.GenerateToken(user.Id);

        return new AuthenticationResponse(UserMapper.ToResponse(user), token);
    }

    public async Task<User> GetAuthenticatedUserAsync()
    {
        var userId = _currentUserService.GetUserId();

        var user = await _repository.GetByIdAsync(userId);

        return user is null
            ? throw new UnauthorizedAccessException("User not authenticated")
            : user;
    }
}