using Blog.DTOs.Category;
using Blog.Entities;

namespace Blog.Mappers;

public static class CategoryMapper
{
    public static Category ToEntity(CategoryRequest request)
    {
        var category = new Category
        {
            Name = request.Name
        };

        return category;
    }

    public static CategoryResponse ToResponse(Category category)
    {
        return new CategoryResponse(
            category.Id,
            category.Name);
    }

    public static void UpdateEntity(
        Category category,
        CategoryRequest request)
    {
        if (request.Name is not null)
        {
            category.Name = request.Name;
        }
    }
}