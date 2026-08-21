using Blog.DTOs.Category;
using Blog.Entities;
using Blog.Exceptions;
using Blog.Repositories.Interfaces;
using Blog.Services;
using Blog.Services.Interfaces;
using Blog.Tests.Factory;
using Moq;

namespace Blog.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _repository;
    private readonly Mock<IAuthorizationService> _authorizationService;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _repository = new Mock<ICategoryRepository>();
        _authorizationService = new Mock<IAuthorizationService>();

        _service = new CategoryService(
            _repository.Object,
            _authorizationService.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateCategorySuccessfully()
    {
        // Arrange
        var request = CategoryFactory.Request();

        _repository.Setup(repository =>
                repository.GetByNameAsync("Movies"))
            .ReturnsAsync((Category?)null);

        _repository.Setup(repository =>
                repository.AddAsync(It.IsAny<Category>()))
            .Callback<Category>(category => category.Id = 1L)
            .Returns(Task.CompletedTask);

        // Act
        var response = await _service.CreateAsync(request);

        // Assert
        var expected = CategoryFactory.Response();
        Assert.Equal(expected, response);

        _authorizationService.Verify(authorization =>
                authorization.ValidateAdminAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByNameAsync("Movies"), Times.Once);

        _repository.Verify(repository =>
                repository.AddAsync(It.Is<Category>(category => category.Name == "Movies")), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowWhenCategoryAlreadyExists()
    {
        // Arrange
        var request = CategoryFactory.Request();
        var category = CategoryFactory.Movies();

        _repository.Setup(repository =>
                repository.GetByNameAsync("Movies"))
            .ReturnsAsync(category);

        // Act / Assert
        await Assert.ThrowsAsync<BadRequestException>(
            () => _service.CreateAsync(request));

        _authorizationService.Verify(authorization =>
                authorization.ValidateAdminAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByNameAsync("Movies"), Times.Once);

        _repository.Verify(repository =>
                repository.AddAsync(It.IsAny<Category>()), Times.Never);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCategorySuccessfully()
    {
        // Arrange
        var request = new CategoryRequest("Books");
        var category = CategoryFactory.Movies();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(category);

        _repository.Setup(repository =>
                repository.GetByNameAsync("Books"))
            .ReturnsAsync((Category?)null);

        _repository.Setup(repository =>
                repository.Update(category))
            .Callback<Category>(category => category.Name = "Books");

        // Act
        var response = await _service.UpdateAsync(1L, request);

        // Assert
        var expected = new CategoryResponse(1L, "Books");
        Assert.Equal(expected, response);

        _authorizationService.Verify(authorization =>
                authorization.ValidateAdminAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _repository.Verify(repository =>
                repository.GetByNameAsync("Books"), Times.Once);

        _repository.Verify(repository =>
                repository.Update(category), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCategoryWithSameName()
    {
        // Arrange
        var request = CategoryFactory.Request();
        var category = CategoryFactory.Movies();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(category);

        _repository.Setup(repository =>
                repository.GetByNameAsync("Movies"))
            .ReturnsAsync(category);

        // Act
        var response = await _service.UpdateAsync(1L, request);

        // Assert
        var expected = CategoryFactory.Response();
        Assert.Equal(expected, response);

        _authorizationService.Verify(authorization =>
                authorization.ValidateAdminAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _repository.Verify(repository =>
                repository.GetByNameAsync("Movies"), Times.Once);

        _repository.Verify(repository =>
                repository.Update(category), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCategoryWithoutChangingName()
    {
        // Arrange
        var request = new CategoryRequest(null!);
        var category = CategoryFactory.Movies();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(category);

        // Act
        var response = await _service.UpdateAsync(1L, request);

        // Assert
        var expected = CategoryFactory.Response();
        Assert.Equal(expected, response);

        _authorizationService.Verify(authorization =>
                authorization.ValidateAdminAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _repository.Verify(repository =>
                repository.GetByNameAsync(It.IsAny<string>()), Times.Never);

        _repository.Verify(repository =>
                repository.Update(category), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowWhenCategoryNameAlreadyExists()
    {
        // Arrange
        var request = CategoryFactory.Request();
        var movies = CategoryFactory.Movies();
        var books = CategoryFactory.Books();

        _repository.Setup(repository =>
                repository.GetByIdAsync(2L))
            .ReturnsAsync(books);

        _repository.Setup(repository =>
                repository.GetByNameAsync("Movies"))
            .ReturnsAsync(movies);

        // Act / Assert
        await Assert.ThrowsAsync<BadRequestException>(
            () => _service.UpdateAsync(2L, request));

        _authorizationService.Verify(authorization =>
                authorization.ValidateAdminAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByIdAsync(2L), Times.Once);

        _repository.Verify(repository =>
                repository.GetByNameAsync("Movies"), Times.Once);

        _repository.Verify(repository =>
                repository.Update(It.IsAny<Category>()), Times.Never);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCategoriesWhenFilterIsNull()
    {
        // Arrange
        var categories = new[]
        {
            CategoryFactory.Movies(),
            CategoryFactory.Books()
        };

        _repository.Setup(repository =>
                repository.GetAllAsync())
            .ReturnsAsync(categories);

        // Act
        var response = (await _service.GetAllAsync(null)).ToList();

        // Assert
        Assert.Equal(2, response.Count);
        Assert.Equal("Movies", response[0].Name);
        Assert.Equal("Books", response[1].Name);

        _repository.Verify(repository =>
                repository.GetAllAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetAllByNameContainingAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCategoriesWhenFilterIsBlank()
    {
        // Arrange
        var categories = new[]
        {
            CategoryFactory.Movies(),
            CategoryFactory.Books()
        };

        _repository.Setup(repository =>
                repository.GetAllAsync())
            .ReturnsAsync(categories);

        // Act
        var response = (await _service.GetAllAsync("   ")).ToList();

        // Assert
        Assert.Equal(2, response.Count);
        Assert.Equal("Movies", response[0].Name);
        Assert.Equal("Books", response[1].Name);

        _repository.Verify(repository =>
                repository.GetAllAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetAllByNameContainingAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterCategoriesByName()
    {
        // Arrange
        var category = CategoryFactory.Movies();

        _repository.Setup(repository =>
                repository.GetAllByNameContainingAsync("mov"))
            .ReturnsAsync([category]);

        // Act
        var response = (await _service.GetAllAsync("mov")).ToList();

        // Assert
        var itemResponse = Assert.Single(response);
        var expected = CategoryFactory.Response();
        Assert.Equal(expected, itemResponse);

        _repository.Verify(repository =>
                repository.GetAllByNameContainingAsync("mov"), Times.Once);

        _repository.Verify(repository =>
                repository.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyListWhenNoCategoriesExist()
    {
        // Arrange
        _repository.Setup(repository =>
                repository.GetAllAsync())
            .ReturnsAsync([]);

        // Act
        var response = (await _service.GetAllAsync(null)).ToList();

        // Assert
        Assert.Empty(response);

        _repository.Verify(repository =>
                repository.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyListWhenNoCategoryMatchesFilter()
    {
        // Arrange
        _repository.Setup(repository =>
                repository.GetAllByNameContainingAsync("mov"))
            .ReturnsAsync([]);

        // Act
        var response = (await _service.GetAllAsync("mov")).ToList();

        // Assert
        Assert.Empty(response);

        _repository.Verify(repository =>
                repository.GetAllByNameContainingAsync("mov"), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCategory()
    {
        // Arrange
        var category = CategoryFactory.Movies();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(category);

        // Act
        var response = await _service.GetByIdAsync(1L);

        // Assert
        var expected = CategoryFactory.Response();
        Assert.Equal(expected, response);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowWhenCategoryNotFound()
    {
        // Arrange
        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync((Category?)null);

        // Act / Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetByIdAsync(1L));

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteCategory()
    {
        // Arrange
        var category = CategoryFactory.Movies();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(category);

        // Act
        await _service.DeleteAsync(1L);

        // Assert
        _authorizationService.Verify(authorization =>
                authorization.ValidateAdminAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _repository.Verify(repository =>
                repository.Delete(category), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowWhenCategoryNotFound()
    {
        // Arrange
        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync((Category?)null);

        // Act / Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _service.DeleteAsync(1L));

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _repository.Verify(repository =>
                repository.Delete(It.IsAny<Category>()), Times.Never);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Never);
    }
}