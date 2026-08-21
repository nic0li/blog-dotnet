using Blog.DTOs.Comment;
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

    public static Post UpdatedPost()
    {
        return Post("I love drama", "Updated content");
    }

    public static PostRequest CreateRequest()
    {
        return new PostRequest("I like drama", "Content", 1L);
    }

    public static PostRequest CreateRequestWithoutTitle()
    {
        return new PostRequest(null, "Content", 1L);
    }

    public static PostRequest CreateRequestWithoutContent()
    {
        return new PostRequest("I like drama", null, 1L);
    }

    public static PostRequest CreateRequestWithoutCategory()
    {
        return new PostRequest("I like drama", "Content", null);
    }

    public static PostRequest UpdateRequest()
    {
        return new PostRequest("I love drama", "Updated content", 1L);
    }

    public static PostResponse Response()
    {
        return Response("I like drama", "Content", []);
    }

    public static PostResponse UpdatedResponse()
    {
        return Response("I love drama", "Updated content", []);
    }

    private static Post Post(string title, string content)
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

    private static PostResponse Response(string title, string content, List<CommentResponse>? comments)
    {
        return new PostResponse(1L,
            title,
            content,
            MockDate,
            MockDate,
            CategoryFactory.Response(),
            UserFactory.ProfileResponse(),
            comments);
    }
}