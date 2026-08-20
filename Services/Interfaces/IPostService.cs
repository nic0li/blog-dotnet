using Blog.DTOs.Post;
using Blog.Entities;

namespace Blog.Services.Interfaces;

public interface IPostService : ICrudService<
    PostResponse,
    PostViewResponse,
    PostCreateRequest>
{
    Task<PostResponse> UpdateAsync(long id, PostUpdateRequest request);

    Task<Post> GetEntityByIdAsync(long id);

    Task<IEnumerable<PostViewResponse>> GetAllAsync(PostFiltersRequest request);

    Task<IEnumerable<PostViewResponse>> GetByUserAsync(long userId);

    Task<IEnumerable<PostViewResponse>>GetByAuthenticatedUserAsync();
}