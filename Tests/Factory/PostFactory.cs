using Blog.DTOs.Post;
using Blog.Entities;

namespace Blog.Tests.Factory;

public static class PostFactory
{
    public static Post Post()
    {
        return Post("I like drama", "Content");
    }

    public static Post UpdatedPost()
    {
        return Post("I love drama", "Updated content");
    }

    public static PostCreateRequest CreateRequest()
    {
        return new PostCreateRequest(
            "I like drama",
            "Content",
            1L);
    }

    public static PostUpdateRequest UpdateRequest()
    {
        return new PostUpdateRequest(
            "I love drama",
            "Updated content",
            1L);
    }

    public static PostResponse Response()
    {
        return Response(
            "I like drama",
            "Content");
    }

    public static PostResponse UpdatedResponse()
    {
        return Response(
            "I love drama", 
            "Updated content");
    }

    public static PostViewResponse ViewResponse()
    {
        return new PostViewResponse(
            1L,
            "I like drama",
            "Content",
            CategoryFactory.Response(),
            UserFactory.ViewResponse(),
            [],
            MockDate,
            MockDate);
    }

    private static Post Post(string title, string content)
    {
        return new Post
        {
            Id = 1L,
            Title = title,
            Content = content,
            Category = CategoryFactory.Movies(),
            User = UserFactory.User(),
            Comments = []
        };
    }

    private static PostResponse Response(string title, string content)
    {
        return new PostResponse(
            1L,
            title,
            content,
            CategoryFactory.Response(),
            UserFactory.Response(),
            MockDate,
            MockDate);
    }

    private static readonly DateTime MockDate = 
        new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}