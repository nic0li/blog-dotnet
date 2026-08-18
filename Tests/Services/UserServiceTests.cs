using Blog.Entities;
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
    public async Task UpdateAsync_ShouldUpdateUserSuccessfully()
    {
        // Arrange
        var request = UserFactory.UpdateRequest();
        var user = UserFactory.User();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(user);

        _authorizationService.Setup(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(user))
            .Returns(Task.CompletedTask);

        _repository.Setup(repository =>
                repository.GetByEmailAsync("mariasilva@email.com"))
            .ReturnsAsync((User?)null);

        // Act
        var response = await _service.UpdateAsync(1L, request);

        // Assert
        var expected = UserFactory.UpdatedResponse();
        Assert.Equal(expected, response);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _authorizationService.Verify(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(user), Times.Once);

        _repository.Verify(repository =>
                repository.GetByEmailAsync(
                    "mariasilva@email.com"), Times.Once);

        _repository.Verify(repository =>
                repository.Update(user), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUserWithNullEmail()
    {
        // Arrange
        var request = UserFactory.UpdateRequestNullEmail();
        var user = UserFactory.User();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(user);

        _authorizationService.Setup(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(user))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _service.UpdateAsync(1L, request);

        // Assert
        var expected = UserFactory.UpdatedResponseSameEmail();
        Assert.Equal(expected, response);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _authorizationService.Verify(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(user), Times.Once);

        _repository.Verify(repository =>
                repository.GetByEmailAsync(It.IsAny<string>()), Times.Never);

        _repository.Verify(repository =>
                repository.Update(user), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUserWithSameEmail()
    {
        // Arrange
        var request = UserFactory.UpdateRequestSameEmail();
        var user = UserFactory.User();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(user);

        _authorizationService.Setup(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(user))
            .Returns(Task.CompletedTask);

        _repository.Setup(repository =>
                repository.GetByEmailAsync("maria@email.com"))
            .ReturnsAsync(user);

        // Act
        var response = await _service.UpdateAsync(1L, request);

        // Assert
        var expected = UserFactory.UpdatedResponseSameEmail();
        Assert.Equal(expected, response);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _authorizationService.Verify(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(user), Times.Once);

        _repository.Verify(repository =>
                repository.GetByEmailAsync("maria@email.com"), Times.Once);

        _repository.Verify(repository =>
                repository.Update(user), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUserWithEmailEmpty()
    {
        // Arrange
        var request = UserFactory.UpdateRequestEmptyEmail();
        var user = UserFactory.User();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(user);

        _authorizationService.Setup(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(user))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _service.UpdateAsync(1L, request);

        // Assert
        var expected = UserFactory.UpdatedResponseSameEmail();
        Assert.Equal(expected, response);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _authorizationService.Verify(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(user), Times.Once);

        _repository.Verify(repository =>
                repository.GetByEmailAsync(It.IsAny<string>()), Times.Never);

        _repository.Verify(repository =>
                repository.Update(user), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUserWithoutEmailField()
    {
        // Arrange
        var request = UserFactory.UpdateRequestWithoutEmail();
        var user = UserFactory.User();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(user);

        _authorizationService.Setup(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(user))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _service.UpdateAsync(1L, request);

        // Assert
        var expected = UserFactory.UpdatedResponseSameEmail();
        Assert.Equal(expected, response);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _authorizationService.Verify(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(user), Times.Once);

        _repository.Verify(repository =>
                repository.GetByEmailAsync(It.IsAny<string>()), Times.Never);

        _repository.Verify(repository =>
                repository.Update(user), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowWhenEmailAlreadyBelongsToAnotherUser()
    {
        // Arrange
        var request = UserFactory.UpdateRequest();
        var user = UserFactory.User();
        var admin = UserFactory.Admin();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(user);

        _authorizationService.Setup(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(user))
            .Returns(Task.CompletedTask);

        _repository.Setup(repository =>
                repository.GetByEmailAsync("mariasilva@email.com"))
            .ReturnsAsync(admin);

        // Act / Assert
        await Assert.ThrowsAsync<BadRequestException>(
            () => _service.UpdateAsync(1L, request));

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _authorizationService.Verify(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(user), Times.Once);

        _repository.Verify(repository =>
        repository.GetByEmailAsync("mariasilva@email.com"), Times.Once);

        _repository.Verify(repository =>
                repository.Update(It.IsAny<User>()), Times.Never);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Never);
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
        var expected = UserFactory.ViewResponse();
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
    public async Task GetMeAsync_ShouldReturnAuthenticatedUser()
    {
        // Arrange
        var user = UserFactory.User();

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        // Act
        var response = await _service.GetMeAsync();

        // Assert
        var expected = UserFactory.Response();
        Assert.Equal(expected, response);

        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateMeAsync_ShouldUpdateAuthenticatedUserSuccessfully()
    {
        // Arrange
        var request = UserFactory.UpdateRequest();
        var user = UserFactory.User();

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        _repository.Setup(repository =>
                repository.GetByEmailAsync("mariasilva@email.com"))
            .ReturnsAsync((User?)null);

        // Act
        var response = await _service.UpdateMeAsync(request);

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
    public async Task UpdateMeAsync_ShouldThrowWhenEmailAlreadyBelongsToAnotherUser()
    {
        // Arrange
        var request = UserFactory.UpdateRequest();
        var user = UserFactory.User();
        var admin = UserFactory.Admin();

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        _repository.Setup(repository =>
                repository.GetByEmailAsync("mariasilva@email.com"))
            .ReturnsAsync(admin);

        // Act / Assert
        await Assert.ThrowsAsync<BadRequestException>(
            () => _service.UpdateMeAsync(request));

        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByEmailAsync("mariasilva@email.com"), Times.Once);

        _repository.Verify(repository =>
                repository.Update(It.IsAny<User>()), Times.Never);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteUser()
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
        await _service.DeleteAsync(1L);

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
    public async Task DeleteAsync_ShouldThrowWhenUserNotFound()
    {
        // Arrange
        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync((User?)null);

        // Act / Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.DeleteAsync(1L));

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _repository.Verify(repository =>
                repository.Delete(It.IsAny<User>()), Times.Never);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteMeAsync_ShouldDeleteAuthenticatedUser()
    {
        // Arrange
        var user = UserFactory.User();

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        // Act
        await _service.DeleteMeAsync();

        // Assert
        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.Delete(user), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }
}