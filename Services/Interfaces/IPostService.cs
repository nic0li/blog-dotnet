using Blog.DTOs.Post;
using Blog.Entities;

namespace Blog.Services.Interfaces;

public interface IPostService : ICrudService<
    PostResponse,
    PostResponse,
    PostCreateRequest>
{
    Task<PostResponse> UpdateAsync(long id, PostUpdateRequest request);

    Task<Post> GetEntityByIdAsync(long id);

    Task<IEnumerable<PostResponse>> GetAllAsync(PostFiltersRequest request);

    Task<IEnumerable<PostResponse>> GetByUserAsync(long userId);

    Task<IEnumerable<PostResponse>>GetByAuthenticatedUserAsync();
}