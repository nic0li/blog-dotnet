using Blog.DTOs.Category;

namespace Blog.Services.Interfaces;

public interface ICategoryService : ICrudService<
    CategoryResponse,
    CategoryResponse,
    CategoryRequest,
    CategoryRequest>
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync(string? name);
}