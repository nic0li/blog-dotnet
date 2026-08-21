using Blog.Entities;
using Blog.Repositories.Interfaces;
using Blog.Services;
using Blog.Services.Interfaces;
using Blog.Tests.Factory;
using Moq;

namespace Blog.Tests.Services;

public class CommentServiceTests
{
    private readonly Mock<ICommentRepository> _repository;
    private readonly Mock<IAuthorizationService> _authorizationService;
    private readonly Mock<IPostService> _postService;
    private readonly CommentService _service;

    public CommentServiceTests()
    {
        _repository = new Mock<ICommentRepository>();
        _authorizationService =
            new Mock<IAuthorizationService>();
        _postService =
            new Mock<IPostService>();

        _service = new CommentService(
            _repository.Object,
            _authorizationService.Object,
            _postService.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateCommentSuccessfully()
    {
        // Arrange
        var request = CommentFactory.Request();
        var comment = CommentFactory.Comment();
        var post = PostFactory.Post();

        _postService.Setup(postService =>
                postService.GetEntityByIdAsync(1L))
            .ReturnsAsync(post);

        _authorizationService.Setup(authorizationService =>
                authorizationService.GetAuthenticatedUserAsync())
            .ReturnsAsync(UserFactory.User());

        _repository.Setup(repository =>
                repository.AddAsync(It.IsAny<Comment>()))
            .Callback<Comment>(entity =>
            {
                entity.Id = 1L;
                entity.CreatedAt = CommentFactory.MockDate;
                entity.UpdatedAt = CommentFactory.MockDate;
            })
            .Returns(Task.CompletedTask);

        // Act
        var response = await _service.CreateAsync(post.Id,request);

        // Assert
        var expected = CommentFactory.Response();
        Assert.Equivalent(expected, response);

        _postService.Verify(postService =>
                postService.GetEntityByIdAsync(1L), Times.Once);

        _authorizationService.Verify(
            authorizationService =>
                authorizationService.GetAuthenticatedUserAsync(), Times.Once);

        _repository.Verify(repository =>
                repository.AddAsync(It.Is<Comment>(entity => entity.Content == "Great post!")), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCommentSuccessfully()
    {
        // Arrange
        var request = CommentFactory.UpdateRequest();
        var comment = CommentFactory.Comment();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(comment);

        _authorizationService.Setup(authorizationService =>
                authorizationService.ValidateOwnerAsync(comment.User))
            .Returns(Task.CompletedTask);

        // Act
        var response = await _service.UpdateAsync(1L, request);

        // Assert
        var expected = CommentFactory.UpdatedResponse();
        Assert.Equivalent(expected, response);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _authorizationService.Verify(authorizationService =>
                authorizationService.ValidateOwnerAsync(comment.User), Times.Once);

        _repository.Verify(repository =>
                repository.Update(comment), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllComments()
    {
        // Arrange
        var comment = CommentFactory.Comment();

        _repository.Setup(repository =>
                repository.GetAllAsync())
            .ReturnsAsync([comment]);

        // Act
        var response = (await _service.GetAllAsync()).ToList();

        // Assert
        var itemResponse = Assert.Single(response);
        var expected = CommentFactory.Response();
        Assert.Equivalent(expected, itemResponse);

        _repository.Verify(repository =>
                repository.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyListWhenNoCommentsExist()
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
    public async Task GetByIdAsync_ShouldReturnComment()
    {
        // Arrange
        var comment = CommentFactory.Comment();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(comment);

        // Act
        var response = await _service.GetByIdAsync(1L);

        // Assert
        var expected = CommentFactory.Response();
        Assert.Equivalent(expected, response);

        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteComment()
    {
        // Arrange
        var comment = CommentFactory.Comment();

        _repository.Setup(repository =>
                repository.GetByIdAsync(1L))
            .ReturnsAsync(comment);

        _authorizationService.Setup(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(comment.User))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1L);

        // Assert
        _repository.Verify(repository =>
                repository.GetByIdAsync(1L), Times.Once);

        _authorizationService.Verify(authorizationService =>
                authorizationService.ValidateOwnerOrAdminAsync(comment.User), Times.Once);

        _repository.Verify(repository =>
                repository.Delete(comment), Times.Once);

        _repository.Verify(repository =>
                repository.SaveChangesAsync(), Times.Once);
    }
}