using Blog.DTOs.Post;
using Blog.Entities;

namespace Blog.Services.Interfaces;

public interface IPostService : ICrudService<
    PostResponse,
    PostViewResponse,
    PostCreateRequest,
    PostUpdateRequest>
{
    Task<Post> GetEntityByIdAsync(long id);

    Task<IEnumerable<PostViewResponse>> GetAllAsync(PostFiltersRequest request);

    Task<IEnumerable<PostViewResponse>> GetByUserAsync(long userId);

    Task<IEnumerable<PostViewResponse>>GetByAuthenticatedUserAsync();
}