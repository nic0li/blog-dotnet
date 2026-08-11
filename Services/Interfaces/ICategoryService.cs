using Blog.DTOs.Category;
using Blog.Entities;

namespace Blog.Services.Interfaces;

public interface ICategoryService : ICrudService<
    CategoryResponse,
    CategoryResponse,
    CategoryRequest,
    CategoryRequest>
{
    Task<Category> GetEntityByIdAsync(long id);

    Task<IEnumerable<CategoryResponse>> GetAllAsync(string? name);
}