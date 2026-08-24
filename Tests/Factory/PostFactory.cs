using Blog.DTOs.Post;
using Blog.Entities;

namespace Blog.Tests.Factory;

public static class PostFactory
{
    public static readonly DateTime MockDate =
        new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static Post Post()
    {
        return Post("I like drama", "Content");
    }

    public static Post Post(string title, string content)
    {
        return EntityPost(title, content);
    }

    public static PostRequest Request()
    {
        return Request("I like drama", "Content", 1L);
    }

    public static PostRequest Request(string? title, string? content, long? categoryId)
    {
        return PostRequest(title, content, categoryId);
    }

    public static PostResponse Response()
    {
        return Response("I like drama", "Content");
    }

    public static PostResponse Response(string title, string content)
    {
        return PostResponse(title, content);
    }

    private static Post EntityPost(string title, string content)
    {
        return new Post
        {
            Id = 1L,
            Title = title,
            Content = content,
            CreatedAt = MockDate,
            UpdatedAt = MockDate,
            Category = CategoryFactory.Movies(),
            User = UserFactory.User(),
            Comments = []
        };
    }

    private static PostRequest PostRequest(string? title, string? content, long? categoryId)
    {
        return new PostRequest(title, content, categoryId);
    }

    private static PostResponse PostResponse(string title, string content)
    {
        return new PostResponse(1L,
            title,
            content,
            MockDate,
            MockDate,
            CategoryFactory.Response(),
            UserFactory.ProfileResponse(),
            []);
    }
}