using Blog.DTOs.Post;
using Blog.Entities;
using Blog.Exceptions;
using Blog.Mappers;
using Blog.Repositories.Interfaces;
using Blog.Services.Interfaces;

namespace Blog.Services;

public class PostService(
    IPostRepository repository,
    IAuthorizationService authorizationService,
    ICategoryService categoryService)
    : EntityService<Post>(repository), IPostService
{
    private readonly IPostRepository _repository = repository;
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private readonly ICategoryService _categoryService = categoryService;

    public async Task<PostResponse> CreateAsync(PostRequest request)
    {
        if (!request.HasTitle || !request.HasContent || !request.HasCategoryId)
        {
            throw new BadRequestException("All fields are required.");
        }

        var post = PostMapper.CreateEntity(request);

        post.Category = await _categoryService.GetEntityByIdAsync(request.CategoryId!.Value);
        post.User = await _authorizationService.GetAuthenticatedUserAsync();

        await _repository.AddAsync(post);
        await _repository.SaveChangesAsync();

        return PostMapper.ToResponse(post);
    }

    public async Task<PostResponse> UpdateAsync(long id, PostRequest request)
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

    public async Task DeleteAsync(long id)
    {
        var post = await GetEntityByIdAsync(id);

        await _authorizationService.ValidateOwnerOrAdminAsync(post.User);

        _repository.Delete(post);

        await _repository.SaveChangesAsync();
    }

    public async Task<PostResponse> GetByIdAsync(long id)
    {
        var post = await GetEntityByIdAsync(id);

        return PostMapper.ToResponse(post);
    }

    public async Task<IEnumerable<PostResponse>> GetAllAsync(PostFiltersRequest request)
    {
        var posts = await FindPostsAsync(request);

        return [.. posts.Select(PostMapper.ToResponse)];
    }

    public async Task<IEnumerable<PostResponse>> GetByUserAsync(long userId)
    {
        var posts = await _repository.GetAllByUserIdAsync(userId);

        return [.. posts.Select(PostMapper.ToResponse)];
    }

    public async Task<IEnumerable<PostResponse>> GetByAuthenticatedUserAsync()
    {
        var user = await _authorizationService.GetAuthenticatedUserAsync();

        return await GetByUserAsync(user.Id);
    }

    private async Task<IEnumerable<Post>> FindPostsAsync(PostFiltersRequest request)
    {
        if (request.HasTitle && request.HasCategory)
        {
            return await _repository
                .GetAllByTitleAndCategoryNameAsync(request.Title!, request.Category!);
        }

        if (request.HasTitle)
        {
            return await _repository
                .GetAllByTitleAsync(request.Title!);
        }

        if (request.HasCategory)
        {
            return await _repository
                .GetAllByCategoryNameAsync(request.Category!);
        }

        return await _repository.GetAllAsync();
    }
}