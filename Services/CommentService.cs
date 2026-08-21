using Blog.DTOs.Comment;
using Blog.Entities;
using Blog.Mappers;
using Blog.Repositories.Interfaces;
using Blog.Services.Interfaces;

namespace Blog.Services;

public class CommentService(
    ICommentRepository repository,
    IAuthorizationService authorizationService,
    IPostService postService)
    : EntityService<Comment>(repository), ICommentService
{
    private readonly ICommentRepository _repository = repository;
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private readonly IPostService _postService = postService;

    public async Task<CommentResponse> CreateAsync(long postId, CommentRequest request)
    {
        var comment = CommentMapper.CreateEntity(request);

        comment.Post = await _postService.GetEntityByIdAsync(postId);
        comment.User = await _authorizationService.GetAuthenticatedUserAsync();

        await _repository.AddAsync(comment);
        await _repository.SaveChangesAsync();

        return CommentMapper.ToResponse(comment);
    }

    public async Task<CommentResponse> UpdateAsync(long id, CommentRequest request)
    {
        var comment = await GetEntityByIdAsync(id);

        await _authorizationService.ValidateOwnerAsync(comment.User);

        CommentMapper.UpdateEntity(comment, request);

        _repository.Update(comment);
        await _repository.SaveChangesAsync();

        return CommentMapper.ToResponse(comment);
    }

    public async Task DeleteAsync(long id)
    {
        var comment = await GetEntityByIdAsync(id);

        await _authorizationService.ValidateOwnerOrAdminAsync(comment.User);

        _repository.Delete(comment);
        await _repository.SaveChangesAsync();
    }

    public async Task<CommentResponse> GetByIdAsync(long id)
    {
        var comment = await GetEntityByIdAsync(id);

        return CommentMapper.ToResponse(comment);
    }

    public async Task<IEnumerable<CommentResponse>> GetAllAsync()
    {
        var comments = await _repository.GetAllAsync();

        return [.. comments.Select(CommentMapper.ToResponse)];
    }
}