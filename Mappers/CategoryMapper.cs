using Blog.DTOs.Category;
using Blog.Entities;

namespace Blog.Mappers;

public static class CategoryMapper
{
    public static Category CreateEntity(CategoryRequest request)
    {
        var category = new Category
        {
            Name = request.Name
        };
        return category;
    }

    public static void UpdateEntity(Category category, CategoryRequest request)
    {
        if (request.Name is not null)
        {
            category.Name = request.Name;
        }
    }

    public static CategoryResponse ToResponse(Category category)
    {
        return new CategoryResponse(category.Id, category.Name);
    }
}