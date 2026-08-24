using Blog.DTOs.User;
using Blog.Entities;
using Blog.Enums;
using Blog.Exceptions;
using Blog.Repositories.Interfaces;
using Blog.Security.Interfaces;
using Blog.Services;
using Blog.Services.Interfaces;
using Blog.Tests.Factory;
using Moq;

namespace Blog.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repository;
    private readonly Mock<IPasswordService> _passwordService;
    private readonly Mock<IAuthorizationService> _authorizationService;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _repository = new Mock<IUserRepository>();
        _passwordService = new Mock<IPasswordService>();
        _authorizationService = new Mock<IAuthorizationService>();

        _service = new UserService(
            _repository.Object,
            _passwordService.Object,
            _authorizationService.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUserSuccessfully()
    {
        // Arrange
        var request = UserFactory.CreateRequest();

        _repository
            .Setup(repository =>
                repository.GetByEmailAsync("maria@email.com"))
            .ReturnsAsync((User?)null);

        _passwordService
            .Setup(passwordService =>
                passwordService.Hash("123456"))
            .Returns("encoded-password");

        _repository
            .Setup(repository =>
                repository.AddAsync(It.IsAny<User>()))
            .Callback<User>(user => user.Id = 1L)
            .Returns(Task.CompletedTask);

        // Act
        var response = await _service.CreateAsync(request);

        // Assert
        var expected = UserFactory.Response();
        Assert.Equal(expected, response);

        _repository.Verify(repository =>
                repository.GetByEmailAsync("maria@email.com"), Times.Once);

        _passwordService.Verify(passwordService =>
                passwordService.Hash("123456"), Times.Once);

        _repository.Verify(repository =>
                repository.AddAsync(It.Is<User>(user => user.Email == "maria@email.com")), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowWhenEmailAlreadyExists()
    {
        // Arrange
        var request = UserFactory.CreateRequest();
        var user = UserFactory.User();

        _repository.Setup(repository =>
                repository.GetByEmailAsync("maria@email.com"))
            .ReturnsAsync(user);

        // Act / Assert
        await Assert.ThrowsAsync<BadRequestException>(
            () => _service.CreateAsync(request));

        _repository.Verify(repository =>
                repository.GetByEmailAsync("maria@email.com"), Times.Once);

        _passwordService.Verify(passwordService =>
                passwordService.Hash(It.IsAny<string>()), Times.Never);

        _repository.Verify(repository =>
                repository.AddAsync(It.IsAny<User>()), Times.Never);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser()
    {
        // Arrange
        var user = UserFactory.User();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(user);

        // Act
        var response = await _service.GetByIdAsync(1L);

        // Assert
        var expected = UserFactory.ProfileResponse();
        Assert.Equal(expected, response);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowWhenUserNotFound()
    {
        // Arrange
        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync((User?)null);

        // Act / Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByIdAsync(1L));

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var user = UserFactory.User();
        var admin = UserFactory.Admin();

        _repository.Setup(repository =>
                repository.GetAllAsync())
            .ReturnsAsync([user, admin]);

        // Act
        var response = (await _service.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, response.Count);
        Assert.Equal("Maria", response[0].Name);
        Assert.Equal("Ana", response[1].Name);

        _repository.Verify(repository =>
                repository.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyListWhenNoUsersExist()
    {
        // Arrange
        _repository.Setup(repository =>
                repository.GetAllAsync())
            .ReturnsAsync([]);

        // Act
        var response = (await _service.GetAllAsync()).ToList();

        // Assert
        Assert.Empty(response);

        _repository.Verify(repository =>
                repository.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ToggleUserRoleAsync_ShouldPromoteUserToAdmin()
    {
        // Arrange
        var authenticatedAdmin = UserFactory.Admin();
        var user = UserFactory.User();

        _authorizationService.Setup(authorizationService =>
                authorizationService.ValidateAdminAsync())
            .Returns(Task.CompletedTask);

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(authenticatedAdmin);

        _repository.Setup(repository =>
                repository.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        // Act
        var response = await _service.ToggleUserRoleAsync(user.Id);

        // Assert
        Assert.Equal(UserRole.Admin, user.Role);

        Assert.Equal(UserRole.Admin, response.Role);

        _authorizationService.Verify(authorizationService =>
                authorizationService.ValidateAdminAsync(), Times.Once);

        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByIdAsync(user.Id), Times.Once);

        _repository.Verify(repository =>
                repository.Update(user), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ToggleUserRoleAsync_ShouldDemoteAdminToUser()
    {
        // Arrange
        var authenticatedAdmin = UserFactory.Admin();
        var targetAdmin = UserFactory.Admin();

        targetAdmin.Id = 3L;

        _authorizationService.Setup(authorizationService =>
                authorizationService.ValidateAdminAsync())
            .Returns(Task.CompletedTask);

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(authenticatedAdmin);

        _repository.Setup(repository =>
                repository.GetByIdAsync(targetAdmin.Id))
            .ReturnsAsync(targetAdmin);

        // Act
        var response = await _service.ToggleUserRoleAsync(targetAdmin.Id);

        // Assert
        Assert.Equal(UserRole.User, targetAdmin.Role);
        Assert.Equal(UserRole.User, response.Role);

        _authorizationService.Verify(authorizationService =>
                authorizationService.ValidateAdminAsync(), Times.Once);

        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByIdAsync(targetAdmin.Id), Times.Once);

        _repository.Verify(repository =>
                repository.Update(targetAdmin), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ToggleUserRoleAsync_ShouldThrowWhenAdminChangesOwnRole()
    {
        // Arrange
        var admin = UserFactory.Admin();

        _authorizationService.Setup(authorizationService =>
                authorizationService.ValidateAdminAsync())
            .Returns(Task.CompletedTask);

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(admin);

        _repository.Setup(repository =>
                repository.GetByIdAsync(admin.Id))
            .ReturnsAsync(admin);

        // Act / Assert
        await Assert.ThrowsAsync<BadRequestException>(
            () => _service.ToggleUserRoleAsync(admin.Id));

        _repository.Verify(repository =>
                repository.Update(It.IsAny<User>()), Times.Never);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task ToggleUserRoleAsync_ShouldThrowWhenUserNotFound()
    {
        // Arrange
        var admin = UserFactory.Admin();

        _authorizationService.Setup(authorizationService =>
                authorizationService.ValidateAdminAsync())
            .Returns(Task.CompletedTask);

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(admin);

        _repository.Setup(repository =>
                repository.GetByIdAsync(999L))
            .ReturnsAsync((User?)null);

        // Act / Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.ToggleUserRoleAsync(999L));

        _repository.Verify(repository =>
                repository.Update(It.IsAny<User>()), Times.Never);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldDeleteUser()
    {
        // Arrange
        var user = UserFactory.User();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(user);

        _authorizationService.Setup(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(user))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteUserAsync(1L);

        // Assert
        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _authorizationService.Verify(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(user), Times.Once);

        _repository.Verify(repository =>
                repository.Delete(user), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldThrowWhenUserNotFound()
    {
        // Arrange
        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync((User?)null);

        // Act / Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.DeleteUserAsync(1L));

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _repository.Verify(repository =>
                repository.Delete(It.IsAny<User>()), Times.Never);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAuthenticatedUserAsync_ShouldReturnAuthenticatedUser()
    {
        // Arrange
        var user = UserFactory.User();

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        // Act
        var response = await _service.GetAuthenticatedAsync();

        // Assert
        var expected = UserFactory.Response();
        Assert.Equal(expected, response);

        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAuthenticatedAsync_ShouldUpdateAuthenticatedUserSuccessfully()
    {
        // Arrange
        var request = UserFactory.UpdateRequest("mariasilva@email.com");
        var user = UserFactory.User();

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        _repository.Setup(repository =>
                repository.GetByEmailAsync("mariasilva@email.com"))
            .ReturnsAsync((User?)null);

        // Act
        var response = await _service.UpdateAuthenticatedAsync(request);

        // Assert
        var expected = UserFactory.UpdatedResponse();
        Assert.Equal(expected, response);

        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByEmailAsync("mariasilva@email.com"), Times.Once);

        _repository.Verify(repository =>
                repository.Update(user), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAuthenticatedAsync_ShouldUpdateAuthenticatedUserWithNullEmail()
    {
        // Arrange
        var request = UserFactory.UpdateRequest(null);
        var user = UserFactory.User();

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        // Act
        var response = await _service.UpdateAuthenticatedAsync(request);

        // Assert
        var expected = UserFactory.UpdatedResponseSameEmail();
        Assert.Equal(expected, response);

        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.Update(user), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAuthenticatedAsync_ShouldUpdateAuthenticatedUserWithSameEmail()
    {
        // Arrange
        var request = UserFactory.UpdateRequest("maria@email.com");
        var user = UserFactory.User();

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        _repository.Setup(repository =>
                repository.GetByEmailAsync("maria@email.com"))
            .ReturnsAsync(user);

        // Act
        var response = await _service.UpdateAuthenticatedAsync(request);

        // Assert
        var expected = UserFactory.UpdatedResponseSameEmail();
        Assert.Equal(expected, response);

        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByEmailAsync("maria@email.com"), Times.Once);

        _repository.Verify(repository =>
                repository.Update(user), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAuthenticatedAsync_ShouldUpdateAuthenticatedUserWithEmailEmpty()
    {
        // Arrange
        var request = UserFactory.UpdateRequest("");
        var user = UserFactory.User();

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        // Act
        var response = await _service.UpdateAuthenticatedAsync(request);

        // Assert
        var expected = UserFactory.UpdatedResponseSameEmail();
        Assert.Equal(expected, response);

        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByEmailAsync(It.IsAny<string>()), Times.Never);

        _repository.Verify(repository =>
                repository.Update(user), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAuthenticatedAsync_ShouldUpdateAuthenticatedUserWithoutEmailField()
    {
        // Arrange
        var request = UserFactory.UpdateRequest();
        var user = UserFactory.User();

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        // Act
        var response = await _service.UpdateAuthenticatedAsync(request);

        // Assert
        var expected = UserFactory.UpdatedResponseSameEmail();
        Assert.Equal(expected, response);

        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByEmailAsync(It.IsAny<string>()), Times.Never);

        _repository.Verify(repository =>
                repository.Update(user), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAuthenticatedAsync_ShouldUpdateAuthenticatedUserWithBlankNamePhotoAndBio()
    {
        // Arrange
        var request = UserFactory.UpdateRequestWithBlankFields();
        var user = UserFactory.User();

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        // Act
        var response = await _service.UpdateAuthenticatedAsync(request);

        // Assert
        var expected = UserFactory.UpdatedResponseWithBlankFields();
        Assert.Equal(expected, response);

        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByEmailAsync(It.IsAny<string>()), Times.Never);

        _repository.Verify(repository =>
                repository.Update(user), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAuthenticatedAsync_ShouldUpdateAuthenticatedUserWithNullNamePhotoAndBio()
    {
        // Arrange
        var request = UserFactory.UpdateRequestWithNullFields();
        var user = UserFactory.User();

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        _repository.Setup(repository =>
                repository.GetByEmailAsync("mariasilva@email.com"))
            .ReturnsAsync((User?)null);

        // Act
        var response = await _service.UpdateAuthenticatedAsync(request);

        // Assert
        var expected = UserFactory.UpdatedResponseWithNullFields();
        Assert.Equal(expected, response);

        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByEmailAsync("mariasilva@email.com"), Times.Once);

        _repository.Verify(repository =>
                repository.Update(user), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAuthenticatedAsync_ShouldThrowWhenEmailAlreadyBelongsToAnotherUser()
    {
        // Arrange
        var request = UserFactory.UpdateRequest("ana@email.com");
        var user = UserFactory.User();
        var admin = UserFactory.Admin();

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        _repository.Setup(repository =>
                repository.GetByEmailAsync("ana@email.com"))
            .ReturnsAsync(admin);

        // Act / Assert
        await Assert.ThrowsAsync<BadRequestException>(
            () => _service.UpdateAuthenticatedAsync(request));

        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByEmailAsync("ana@email.com"), Times.Once);

        _repository.Verify(repository =>
                repository.Update(It.IsAny<User>()), Times.Never);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAuthenticatedPasswordAsync_ShouldUpdatePasswordSuccessfully()
    {
        // Arrange
        var request = new UserPasswordUpdateRequest("123456", "654321");

        var user = UserFactory.User();
        var currentPasswordHash = user.Password;

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        _passwordService.Setup(passwordService =>
                passwordService.Verify(request.CurrentPassword, currentPasswordHash))
            .Returns(true);

        _passwordService.Setup(passwordService =>
                passwordService.Hash(request.NewPassword))
            .Returns("new-hashed-password");

        // Act
        await _service.UpdateAuthenticatedPasswordAsync(request);

        // Assert
        Assert.Equal("new-hashed-password", user.Password);

        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);

        _passwordService.Verify(passwordService =>
                passwordService.Verify(request.CurrentPassword, currentPasswordHash), Times.Once);

        _passwordService.Verify(passwordService =>
                passwordService.Hash(request.NewPassword), Times.Once);

        _repository.Verify(repository =>
                repository.Update(user), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAuthenticatedPasswordAsync_ShouldThrowWhenCurrentPasswordIsInvalid()
    {
        // Arrange
        var request = new UserPasswordUpdateRequest("wrong-password", "654321");

        var user = UserFactory.User();
        var currentPasswordHash = user.Password;

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        _passwordService.Setup(passwordService =>
                passwordService.Verify(request.CurrentPassword, currentPasswordHash))
            .Returns(false);

        // Act / Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.UpdateAuthenticatedPasswordAsync(request));

        _passwordService.Verify(passwordService =>
                passwordService.Verify(request.CurrentPassword, currentPasswordHash), Times.Once);

        _passwordService.Verify(passwordService =>
                passwordService.Hash(It.IsAny<string>()), Times.Never);

        _repository.Verify(repository =>
                repository.Update(It.IsAny<User>()), Times.Never);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAuthenticatedAsync_ShouldDeleteAuthenticatedUser()
    {
        // Arrange
        var user = UserFactory.User();

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        // Act
        await _service.DeleteAuthenticatedAsync();

        // Assert
        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.Delete(user), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }
}