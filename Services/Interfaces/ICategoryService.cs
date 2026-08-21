using Blog.DTOs.Category;
using Blog.Entities;

namespace Blog.Services.Interfaces;

public interface ICategoryService : IEntityService<Category>
{
    Task<CategoryResponse> CreateAsync(CategoryRequest request);

    Task<CategoryResponse> UpdateAsync(long id, CategoryRequest request);

    Task DeleteAsync(long id);

    Task<CategoryResponse> GetByIdAsync(long id);

    Task<IEnumerable<CategoryResponse>> GetAllAsync(string? name);
}