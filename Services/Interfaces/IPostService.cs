using Blog.DTOs.Post;
using Blog.Entities;

namespace Blog.Services.Interfaces;

public interface IPostService : IEntityService<Post>
{
    Task<PostResponse> CreateAsync(PostRequest request);

    Task<PostResponse> UpdateAsync(long id, PostRequest request);

    Task DeleteAsync(long id);

    Task<PostResponse> GetByIdAsync(long id);

    Task<IEnumerable<PostResponse>> GetAllAsync(PostFiltersRequest request);

    Task<IEnumerable<PostResponse>> GetByUserAsync(long userId);

    Task<IEnumerable<PostResponse>>GetByAuthenticatedUserAsync();
}