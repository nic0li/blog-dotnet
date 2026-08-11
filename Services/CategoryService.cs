using Blog.DTOs.Category;
using Blog.Entities;
using Blog.Exceptions;
using Blog.Mappers;
using Blog.Repositories.Interfaces;
using Blog.Services.Interfaces;

namespace Blog.Services;

public class CategoryService(
    ICategoryRepository repository,
    IAuthorizationService authorizationService) : CrudService<
    Category,
    CategoryResponse,
    CategoryResponse,
    CategoryRequest,
    CategoryRequest>(repository),
    ICategoryService
{
    private readonly ICategoryRepository _repository = repository;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    public override async Task<CategoryResponse> CreateAsync(CategoryRequest request)
    {
        await _authorizationService.ValidateAdminAsync();

        await ValidateUniqueNameAsync(request.Name, null);

        var category = CategoryMapper.ToEntity(request);

        await _repository.AddAsync(category);
        await _repository.SaveChangesAsync();

        return CategoryMapper.ToResponse(category);
    }

    public override async Task<CategoryResponse> UpdateAsync(long id, CategoryRequest request)
    {
        await _authorizationService.ValidateAdminAsync();

        var category = await GetEntityByIdAsync(id);

        await ValidateUniqueNameAsync(request.Name, id);

        CategoryMapper.UpdateEntity(category, request);

        _repository.Update(category);

        await _repository.SaveChangesAsync();

        return CategoryMapper.ToResponse(category);
    }

    public override async Task DeleteAsync(long id)
    {
        await _authorizationService.ValidateAdminAsync();

        var category = await GetEntityByIdAsync(id);

        _repository.Delete(category);

        await _repository.SaveChangesAsync();
    }

    public override async Task<CategoryResponse> GetByIdAsync(long id)
    {
        var category = await GetEntityByIdAsync(id);

        return CategoryMapper.ToResponse(category);
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync(string? name)
    {
        var categories = await FindCategoriesAsync(name);

        return [.. categories.Select(CategoryMapper.ToResponse)];
    }

    private async Task<IEnumerable<Category>> FindCategoriesAsync(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return await _repository.GetAllAsync();
        }

        return await _repository.GetAllByNameContainingAsync(name);
    }

    private async Task ValidateUniqueNameAsync(string? name, long? categoryId)
    {
        if (name is null)
        {
            return;
        }

        var categoryByName = await _repository.GetByNameAsync(name);

        var categoryAlreadyExists = categoryByName is not null;

        var isDifferent = categoryAlreadyExists && categoryByName!.Id != categoryId;

        if (categoryAlreadyExists && isDifferent)
        {
            throw new BadRequestException("Category already exists");
        }
    }
}