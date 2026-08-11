using Blog.DTOs.Post;
using Blog.Entities;
using Blog.Mappers;
using Blog.Repositories.Interfaces;
using Blog.Services.Interfaces;

namespace Blog.Services;

public class PostService(
    IPostRepository repository,
    IAuthorizationService authorizationService,
    ICategoryService categoryService) : CrudService<
    Post,
    PostResponse,
    PostViewResponse,
    PostCreateRequest,
    PostUpdateRequest>(repository),
    IPostService
{
    private readonly IPostRepository _repository = repository;
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private readonly ICategoryService _categoryService = categoryService;

    public override async Task<PostResponse> CreateAsync(
        PostCreateRequest request)
    {
        var post = PostMapper.CreateEntity(request);

        post.Category = await _categoryService.GetEntityByIdAsync(request.CategoryId);

        post.User = await _authorizationService.GetAuthenticatedUserAsync();

        await _repository.AddAsync(post);
        await _repository.SaveChangesAsync();

        return PostMapper.ToResponse(post);
    }

    public override async Task<PostResponse> UpdateAsync(long id, PostUpdateRequest request)
    {
        var post = await GetEntityByIdAsync(id);

        await _authorizationService.ValidateOwnerAsync(post.User);

        PostMapper.UpdateEntity(post, request);

        if (request.CategoryId is not null)
        {
            post.Category = await _categoryService.GetEntityByIdAsync(request.CategoryId.Value);
        }

        _repository.Update(post);

        await _repository.SaveChangesAsync();

        return PostMapper.ToResponse(post);
    }

    public override async Task DeleteAsync(long id)
    {
        var post = await GetEntityByIdAsync(id);

        await _authorizationService.ValidateOwnerOrAdminAsync(post.User);

        _repository.Delete(post);

        await _repository.SaveChangesAsync();
    }

    public override async Task<PostViewResponse> GetByIdAsync(long id)
    {
        var post = await GetEntityByIdAsync(id);

        return PostMapper.ToViewResponse(post);
    }

    public async Task<IEnumerable<PostViewResponse>> GetAllAsync(PostFiltersRequest request)
    {
        var posts = await FindPostsAsync(request);

        return [.. posts.Select(PostMapper.ToViewResponse)];
    }

    public async Task<IEnumerable<PostViewResponse>> GetByUserAsync(long userId)
    {
        var posts = await _repository.GetAllByUserIdAsync(userId);

        return [.. posts.Select(PostMapper.ToViewResponse)];
    }

    public async Task<IEnumerable<PostViewResponse>> GetByAuthenticatedUserAsync()
    {
        var user = await _authorizationService.GetAuthenticatedUserAsync();

        return await GetByUserAsync(user.Id);
    }

    private async Task<IEnumerable<Post>> FindPostsAsync(PostFiltersRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Title) &&
            !string.IsNullOrWhiteSpace(request.Category))
        {
            return await _repository
                .GetAllByTitleContainingAndCategoryNameContainingAsync(
                    request.Title,
                    request.Category);
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            return await _repository.GetAllByTitleContainingAsync(request.Title);
        }

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            return await _repository.GetAllByCategoryNameContainingAsync(request.Category);
        }

        return await _repository.GetAllAsync();
    }
}