using Blog.DTOs.Category;
using Blog.Entities;

namespace Blog.Services.Interfaces;

public interface ICategoryService : ICrudService<
    CategoryResponse,
    CategoryResponse,
    CategoryRequest>
{
    Task<CategoryResponse> UpdateAsync(long id, CategoryRequest request);

    Task<Category> GetEntityByIdAsync(long id);

    Task<IEnumerable<CategoryResponse>> GetAllAsync(string? name);
}