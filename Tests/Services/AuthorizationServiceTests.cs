using Blog.Exceptions;
using Blog.Services;
using Blog.Services.Interfaces;
using Blog.Tests.Factory;
using Moq;

namespace Blog.Tests.Services;

public class AuthorizationServiceTests
{
    private readonly Mock<IAuthenticationService> _authenticationService;
    private readonly AuthorizationService _service;

    public AuthorizationServiceTests()
    {
        _authenticationService = new Mock<IAuthenticationService>();
        _service = new AuthorizationService(_authenticationService.Object);
    }

    [Fact]
    public async Task GetAuthenticatedUserAsync_ShouldReturnAuthenticatedUser()
    {
        // Arrange
        var user = UserFactory.User();

        _authenticationService.Setup(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        // Act
        var response = await _service.GetAuthenticatedUserAsync();

        // Assert
        Assert.Equal(user, response);

        _authenticationService.Verify(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync(), Times.Once);
    }

    [Fact]
    public async Task IsOwnerAsync_ShouldReturnTrueWhenUserIsOwner()
    {
        // Arrange
        var user = UserFactory.User();

        _authenticationService.Setup(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        // Act
        var response = await _service.IsOwnerAsync(user);

        // Assert
        Assert.True(response);

        _authenticationService.Verify(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync(), Times.Once);
    }

    [Fact]
    public async Task IsOwnerAsync_ShouldReturnFalseWhenUserIsNotOwner()
    {
        // Arrange
        var user = UserFactory.User();
        var admin = UserFactory.Admin();

        _authenticationService.Setup(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        // Act
        var response = await _service.IsOwnerAsync(admin);

        // Assert
        Assert.False(response);

        _authenticationService.Verify(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync(), Times.Once);
    }

    [Fact]
    public async Task IsAdminAsync_ShouldReturnTrueWhenUserIsAdmin()
    {
        // Arrange
        var admin = UserFactory.Admin();

        _authenticationService.Setup(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(admin);

        // Act
        var response = await _service.IsAdminAsync();

        // Assert
        Assert.True(response);

        _authenticationService.Verify(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync(), Times.Once);
    }

    [Fact]
    public async Task IsAdminAsync_ShouldReturnFalseWhenUserIsNotAdmin()
    {
        // Arrange
        var user = UserFactory.User();

        _authenticationService.Setup(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        // Act
        var response = await _service.IsAdminAsync();

        // Assert
        Assert.False(response);

        _authenticationService.Verify(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync(), Times.Once);
    }

    [Fact]
    public async Task ValidateOwnerAsync_ShouldSucceedWhenUserIsOwner()
    {
        // Arrange
        var user = UserFactory.User();

        _authenticationService.Setup(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        // Act
        await _service.ValidateOwnerAsync(user);

        // Assert
        _authenticationService.Verify(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync(), Times.Once);
    }

    [Fact]
    public async Task ValidateOwnerAsync_ShouldThrowWhenUserIsNotOwner()
    {
        // Arrange
        var user = UserFactory.User();
        var admin = UserFactory.Admin();

        _authenticationService.Setup(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        // Act / Assert
        await Assert.ThrowsAsync<ForbiddenException>(
            () => _service.ValidateOwnerAsync(admin));

        _authenticationService.Verify(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync(), Times.Once);
    }

    [Fact]
    public async Task ValidateAdminAsync_ShouldSucceedWhenUserIsAdmin()
    {
        // Arrange
        var admin = UserFactory.Admin();

        _authenticationService.Setup(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(admin);

        // Act
        await _service.ValidateAdminAsync();

        // Assert
        _authenticationService.Verify(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync(), Times.Once);
    }

    [Fact]
    public async Task ValidateAdminAsync_ShouldThrowWhenUserIsNotAdmin()
    {
        // Arrange
        var user = UserFactory.User();

        _authenticationService.Setup(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        // Act / Assert
        await Assert.ThrowsAsync<ForbiddenException>(
            () => _service.ValidateAdminAsync());

        _authenticationService.Verify(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync(), Times.Once);
    }

    [Fact]
    public async Task ValidateOwnerOrAdminAsync_ShouldSucceedWhenUserIsOwner()
    {
        // Arrange
        var user = UserFactory.User();

        _authenticationService.Setup(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        // Act
        await _service.ValidateOwnerOrAdminAsync(user);

        // Assert
        _authenticationService.Verify(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync(), Times.Once);
    }

    [Fact]
    public async Task ValidateOwnerOrAdminAsync_ShouldSucceedWhenUserIsAdmin()
    {
        // Arrange
        var user = UserFactory.User();
        var admin = UserFactory.Admin();

        _authenticationService.Setup(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(admin);

        // Act
        await _service.ValidateOwnerOrAdminAsync(user);

        // Assert
        _authenticationService.Verify(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync(), Times.Once);
    }

    [Fact]
    public async Task ValidateOwnerOrAdminAsync_ShouldThrowWhenUserIsNotOwnerOrAdmin()
    {
        // Arrange
        var user = UserFactory.User();
        var admin = UserFactory.Admin();

        _authenticationService.Setup(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        // Act / Assert
        await Assert.ThrowsAsync<ForbiddenException>(
            () => _service.ValidateOwnerOrAdminAsync(admin));

        _authenticationService.Verify(authenticationService =>
                authenticationService.GetAuthenticatedUserAsync(), Times.Once);
    }
}