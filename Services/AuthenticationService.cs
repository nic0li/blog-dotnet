using Blog.DTOs.Authentication;
using Blog.Entities;
using Blog.Mappers;
using Blog.Repositories;
using Blog.Security.Interfaces;
using Blog.Services.Interfaces;

namespace Blog.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _repository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;
    private readonly ICurrentUserService _currentUserService;

    public AuthenticationService(
        IUserRepository repository,
        IJwtService jwtService,
        IPasswordService passwordService,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _jwtService = jwtService;
        _passwordService = passwordService;
        _currentUserService = currentUserService;
    }

    public async Task<LoginResponse> AuthenticateAsync(
        LoginRequest request)
    {
        var user = await _repository.GetByEmailAsync(request.Login);

        if (user is null ||
            !_passwordService.Verify(
                request.Password,
                user.Password))
        {
            throw new UnauthorizedAccessException(
                "Invalid credentials");
        }

        var token = _jwtService.GenerateToken(user.Id);

        return new LoginResponse(
            UserMapper.ToResponse(user),
            token);
    }

    public async Task<User> GetAuthenticatedUserAsync()
    {
        var userId = _currentUserService.GetUserId();

        var user = await _repository.GetByIdAsync(userId);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "User not authenticated");
        }

        return user;
    }
}