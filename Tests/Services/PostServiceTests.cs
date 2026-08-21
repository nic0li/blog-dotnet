using Blog.DTOs.Post;
using Blog.Entities;
using Blog.Exceptions;
using Blog.Repositories.Interfaces;
using Blog.Services;
using Blog.Services.Interfaces;
using Blog.Tests.Factory;
using Moq;

namespace Blog.Tests.Services;

public class PostServiceTests
{
    private readonly Mock<IPostRepository> _repository;
    private readonly Mock<IAuthorizationService> _authorizationService;
    private readonly Mock<ICategoryService> _categoryService;
    private readonly PostService _service;

    public PostServiceTests()
    {
        _repository = new Mock<IPostRepository>();
        _authorizationService = new Mock<IAuthorizationService>();
        _categoryService = new Mock<ICategoryService>();

        _service = new PostService(
            _repository.Object,
            _authorizationService.Object,
            _categoryService.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreatePostSuccessfully()
    {
        // Arrange
        var request = PostFactory.CreateRequest();
        var post = PostFactory.Post();

        _categoryService.Setup(categoryService =>
                categoryService.GetEntityByIdAsync(1L))
            .ReturnsAsync(CategoryFactory.Movies());

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(UserFactory.User());

        _repository.Setup(repository =>
                repository.AddAsync(It.IsAny<Post>()))
            .Callback<Post>(entity =>
            {
                entity.Id = 1L;
                entity.CreatedAt = PostFactory.MockDate;
                entity.UpdatedAt = PostFactory.MockDate;
            })
            .Returns(Task.CompletedTask);

        // Act
        var response = await _service.CreateAsync(request);

        // Assert
        var expected = PostFactory.Response();
        Assert.Equivalent(expected, response);

        _categoryService.Verify(categoryService =>
                categoryService.GetEntityByIdAsync(1L), Times.Once);

        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.AddAsync(It.Is<Post>(entity =>
                        entity.Title == "I like drama" &&
                        entity.Content == "Content")), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowWhenTitleIsMissing()
    {
        // Arrange
        var request = PostFactory.CreateRequestWithoutTitle();

        // Act / Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _service.CreateAsync(request));

        Assert.Equal("All fields are required.", exception.Message);

        _repository.Verify(repository => 
        repository.AddAsync(It.IsAny<Post>()), Times.Never);

        _categoryService.VerifyNoOtherCalls();
        _authorizationService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowWhenContentIsMissing()
    {
        // Arrange
        var request = PostFactory.CreateRequestWithoutContent();

        // Act / Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _service.CreateAsync(request));

        Assert.Equal("All fields are required.", exception.Message);

        _repository.Verify(repository => 
        repository.AddAsync(It.IsAny<Post>()), Times.Never);

        _categoryService.VerifyNoOtherCalls();
        _authorizationService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowWhenCategoryIsMissing()
    {
        // Arrange
        var request = PostFactory.CreateRequestWithoutCategory();

        // Act / Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _service.CreateAsync(request));

        Assert.Equal("All fields are required.", exception.Message);

        _repository.Verify(repository => 
        repository.AddAsync(It.IsAny<Post>()), Times.Never);

        _categoryService.VerifyNoOtherCalls();
        _authorizationService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdatePostSuccessfully()
    {
        // Arrange
        var request = PostFactory.UpdateRequest();
        var post = PostFactory.Post();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(post);

        _authorizationService.Setup(authorizationService =>
                authorizationService.ValidateOwnerAsync(post.User))
            .Returns(Task.CompletedTask);

        _categoryService.Setup(categoryService =>
                categoryService.GetEntityByIdAsync(1L))
            .ReturnsAsync(CategoryFactory.Movies());

        // Act
        var response = await _service.UpdateAsync(1L, request);

        // Assert
        var expected = PostFactory.UpdatedResponse();
        Assert.Equivalent(expected, response);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _authorizationService.Verify(authorizationService =>
                authorizationService.ValidateOwnerAsync(post.User), Times.Once);

        _categoryService.Verify(categoryService =>
                categoryService.GetEntityByIdAsync(1L), Times.Once);

        _repository.Verify(repository =>
                repository.Update(post), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdatePostWithoutChangingCategory()
    {
        // Arrange
        var request = new PostRequest(null, null, null);

        var post = PostFactory.Post();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(post);

        _authorizationService.Setup(authorizationService =>
                authorizationService.ValidateOwnerAsync(post.User))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _service.UpdateAsync(1L, request);

        // Assert
        var expected = PostFactory.Response();
        Assert.Equivalent(expected, response);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _authorizationService.Verify(authorizationService =>
                authorizationService.ValidateOwnerAsync(post.User), Times.Once);

        _categoryService.Verify(categoryService =>
                categoryService.GetEntityByIdAsync(It.IsAny<long>()), Times.Never);

        _repository.Verify(repository =>
                repository.Update(post), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPosts()
    {
        // Arrange
        var post = PostFactory.Post();

        _repository.Setup(repository =>
                repository.GetAllAsync())
            .ReturnsAsync([post]);

        // Act
        var response = (await _service.GetAllAsync(
            new PostFiltersRequest(null, null))).ToList();

        // Assert
        var itemResponse = Assert.Single(response);
        var expected = PostFactory.Response();
        Assert.Equivalent(expected, itemResponse);

        _repository.Verify(repository =>
                repository.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterPostsByTitle()
    {
        // Arrange
        var post = PostFactory.Post();

        _repository.Setup(repository =>
                repository.GetAllByTitleAsync("Like"))
            .ReturnsAsync([post]);

        // Act
        var response = (await _service.GetAllAsync(
            new PostFiltersRequest("Like", null))).ToList();

        // Assert
        var itemResponse = Assert.Single(response);
        var expected = PostFactory.Response();
        Assert.Equivalent(expected, itemResponse);

        _repository.Verify(repository =>
                repository.GetAllByTitleAsync("Like"), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterPostsByCategory()
    {
        // Arrange
        var post = PostFactory.Post();

        _repository.Setup(repository =>
                repository.GetAllByCategoryNameAsync("Movies"))
            .ReturnsAsync([post]);

        // Act
        var response = (await _service.GetAllAsync(
            new PostFiltersRequest(null, "Movies"))).ToList();

        // Assert
        var itemResponse = Assert.Single(response);
        var expected = PostFactory.Response();
        Assert.Equivalent(expected, itemResponse);

        _repository.Verify(repository =>
                repository.GetAllByCategoryNameAsync("Movies"), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterPostsByTitleAndCategory()
    {
        // Arrange
        var post = PostFactory.Post();

        _repository.Setup(repository =>
                repository.GetAllByTitleAndCategoryNameAsync("Like", "Movies"))
            .ReturnsAsync([post]);

        // Act
        var response = (await _service.GetAllAsync(
            new PostFiltersRequest("Like", "Movies"))).ToList();

        // Assert
        var itemResponse = Assert.Single(response);
        var expected = PostFactory.Response();
        Assert.Equivalent(expected, itemResponse);

        _repository.Verify(repository =>
                repository.GetAllByTitleAndCategoryNameAsync("Like", "Movies"), Times.Once);
    }

    [Fact]
    public async Task GetByUserAsync_ShouldReturnPostsByUser()
    {
        // Arrange
        var post = PostFactory.Post();
        var comment = CommentFactory.Comment();
        post.Comments.Add(comment);

        _repository.Setup(repository =>
                repository.GetAllByUserIdAsync(1L))
            .ReturnsAsync([post]);

        // Act
        var response = (await _service.GetByUserAsync(1L)).ToList();

        // Assert
        var itemResponse = Assert.Single(response);
        var commentWithoutPost = Assert.Single(itemResponse.Comments!);
        var expected = PostFactory.Response();
        Assert.Equivalent(expected, itemResponse);
        Assert.Null(commentWithoutPost.Post);

        _repository.Verify(repository =>
                repository.GetAllByUserIdAsync(1L), Times.Once);
    }

    [Fact]
    public async Task GetByAuthenticatedUserAsync_ShouldReturnPostsByAuthenticatedUser()
    {
        // Arrange
        var post = PostFactory.Post();
        var user = UserFactory.User();

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(user);

        _repository.Setup(repository =>
                repository.GetAllByUserIdAsync(1L))
            .ReturnsAsync([post]);

        // Act
        var response = (await _service.GetByAuthenticatedUserAsync()).ToList();

        // Assert
        var itemResponse = Assert.Single(response);
        var expected = PostFactory.Response();
        Assert.Equivalent(expected, itemResponse);

        _authorizationService.Verify(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.GetAllByUserIdAsync(1L), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPost()
    {
        // Arrange
        var post = PostFactory.Post();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(post);

        // Act
        var response = await _service.GetByIdAsync(1L);

        // Assert
        var expected = PostFactory.Response();
        Assert.Equivalent(expected, response);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeletePost()
    {
        // Arrange
        var post = PostFactory.Post();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(post);

        _authorizationService.Setup(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(post.User))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1L);

        // Assert
        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _authorizationService.Verify(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(post.User), Times.Once);

        _repository.Verify(repository =>
                repository.Delete(post), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }
}