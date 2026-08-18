using Blog.DTOs.Category;
using Blog.Entities;

namespace Blog.Tests.Factory;

public static class CategoryFactory
{
    public static Category Movies()
    {
        return Category(1L, "Movies");
    }

    public static Category Books()
    {
        return Category(2L, "Books");
    }

    public static CategoryRequest Request()
    {
        return new CategoryRequest("Movies");
    }

    public static CategoryResponse Response()
    {
        return new CategoryResponse(1L, "Movies");
    }

    private static Category Category(long id, string name)
    {
        return new Category
        {
            Id = id,
            Name = name
        };
    }
}