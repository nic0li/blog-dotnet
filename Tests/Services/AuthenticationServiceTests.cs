using Blog.DTOs.Authentication;
using Blog.Entities;
using Blog.Repositories.Interfaces;
using Blog.Security.Interfaces;
using Blog.Services;
using Blog.Tests.Factory;
using Moq;

namespace Blog.Tests.Services;

public class AuthenticationServiceTests
{
    private readonly Mock<IUserRepository> _repository;
    private readonly Mock<IJwtService> _jwtService;
    private readonly Mock<IPasswordService> _passwordService;
    private readonly Mock<ICurrentUserService> _currentUserService;
    private readonly AuthenticationService _service;

    public AuthenticationServiceTests()
    {
        _repository = new Mock<IUserRepository>();
        _jwtService = new Mock<IJwtService>();
        _passwordService = new Mock<IPasswordService>();
        _currentUserService = new Mock<ICurrentUserService>();

        _service = new AuthenticationService(
            _repository.Object,
            _jwtService.Object,
            _passwordService.Object,
            _currentUserService.Object);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldAuthenticateSuccessfully()
    {
        // Arrange
        var request = new LoginRequest("maria@email.com", "123456");
        var user = UserFactory.User();

        _repository.Setup(repository =>
                repository.GetByEmailAsync("maria@email.com"))
            .ReturnsAsync(user);

        _passwordService.Setup(passwordService =>
                passwordService.Verify("123456", user.Password))
            .Returns(true);

        _jwtService.Setup(jwtService =>
                jwtService.GenerateToken(1L))
            .Returns("jwt-token");

        // Act
        var response = await _service.AuthenticateAsync(request);

        // Assert
        Assert.Equal(1L, response.User.Id);
        Assert.Equal("maria@email.com", response.User.Email);
        Assert.Equal("jwt-token", response.Token);

        _repository.Verify(repository =>
                repository.GetByEmailAsync("maria@email.com"), Times.Once);

        _passwordService.Verify(passwordService =>
                passwordService.Verify("123456",user.Password), Times.Once);

        _jwtService.Verify(jwtService =>
                jwtService.GenerateToken(1L), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldThrowWhenCredentialsAreInvalid()
    {
        // Arrange
        var request = new LoginRequest("maria@email.com", "123456");
        var user = UserFactory.User();

        _repository.Setup(repository =>
                repository.GetByEmailAsync("maria@email.com"))
            .ReturnsAsync(user);

        _passwordService.Setup(passwordService =>
                passwordService.Verify("123456", user.Password))
            .Returns(false);

        // Act / Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.AuthenticateAsync(request));

        _repository.Verify(repository =>
                repository.GetByEmailAsync("maria@email.com"), Times.Once);

        _passwordService.Verify(passwordService =>
                passwordService.Verify("123456", user.Password), Times.Once);

        _jwtService.Verify(jwtService =>
                jwtService.GenerateToken(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldThrowWhenUserDoesNotExist()
    {
        // Arrange
        var request = new LoginRequest("maria@email.com", "123456");

        _repository.Setup(repository =>
                repository.GetByEmailAsync("maria@email.com"))
            .ReturnsAsync((User?)null);

        // Act / Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.AuthenticateAsync(request));

        _passwordService.Verify(passwordService =>
                passwordService.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        _jwtService.Verify(jwtService =>
                jwtService.GenerateToken(It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task GetAuthenticatedUserAsync_ShouldReturnAuthenticatedUser()
    {
        // Arrange
        var user = UserFactory.User();

        _currentUserService.Setup(currentUserService =>
                currentUserService.GetUserId())
            .Returns(user.Id);

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(user);

        // Act
        var response = await _service.GetAuthenticatedUserAsync();

        // Assert
        Assert.Equal(1L, response.Id);
        Assert.Equal("maria@email.com", response.Email);

        _currentUserService.Verify(currentUserService =>
                currentUserService.GetUserId(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);
    }

    [Fact]
    public async Task GetAuthenticatedUserAsync_ShouldThrowWhenUserDoesNotExist()
    {
        // Arrange
        _currentUserService.Setup(currentUserService =>
                currentUserService.GetUserId())
            .Returns(1L);

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync((User?)null);

        // Act / Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.GetAuthenticatedUserAsync());

        _currentUserService.Verify(currentUserService =>
                currentUserService.GetUserId(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);
    }
}