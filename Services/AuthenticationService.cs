using Blog.DTOs.Authentication;
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

    public AuthenticationService(
        IUserRepository repository,
        IJwtService jwtService,
        IPasswordService passwordService)
    {
        _repository = repository;
        _jwtService = jwtService;
        _passwordService = passwordService;
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
}