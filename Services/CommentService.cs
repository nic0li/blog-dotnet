using Blog.DTOs.Comment;
using Blog.Entities;
using Blog.Mappers;
using Blog.Repositories.Interfaces;
using Blog.Services.Interfaces;

namespace Blog.Services;

public class CommentService(
    ICommentRepository repository,
    IAuthorizationService authorizationService,
    IPostService postService) : CrudService<
    Comment,
    CommentResponse,
    CommentViewResponse,
    CommentCreateRequest,
    CommentUpdateRequest>(repository),
    ICommentService
{
    private readonly ICommentRepository _repository = repository;
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private readonly IPostService _postService = postService;

    public override async Task<CommentResponse> CreateAsync(CommentCreateRequest request)
    {
        var comment = CommentMapper.CreateEntity(request);

        comment.Post = await _postService.GetEntityByIdAsync(request.PostId);

        comment.User = await _authorizationService.GetAuthenticatedUserAsync();

        await _repository.AddAsync(comment);
        await _repository.SaveChangesAsync();

        return CommentMapper.ToResponse(comment);
    }

    public async Task<CommentResponse> UpdateAsync(long id, CommentUpdateRequest request)
    {
        var comment = await GetEntityByIdAsync(id);

        await _authorizationService.ValidateOwnerAsync(comment.User);

        CommentMapper.UpdateEntity(comment, request);

        _repository.Update(comment);

        await _repository.SaveChangesAsync();

        return CommentMapper.ToResponse(comment);
    }

    public override async Task DeleteAsync(long id)
    {
        var comment = await GetEntityByIdAsync(id);

        await _authorizationService.ValidateOwnerOrAdminAsync(comment.User);

        _repository.Delete(comment);

        await _repository.SaveChangesAsync();
    }

    public override async Task<CommentViewResponse> GetByIdAsync(long id)
    {
        var comment = await GetEntityByIdAsync(id);

        return CommentMapper.ToViewResponse(comment);
    }

    public async Task<IEnumerable<CommentViewResponse>> GetAllAsync()
    {
        var comments = await _repository.GetAllAsync();

        return [.. comments.Select(CommentMapper.ToViewResponse)];
    }
}