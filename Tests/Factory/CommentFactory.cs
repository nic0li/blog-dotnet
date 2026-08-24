using Blog.DTOs.Comment;
using Blog.Entities;

namespace Blog.Tests.Factory;

public static class CommentFactory
{
    public static readonly DateTime MockDate =
        new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static Comment Comment()
    {
        return EntityComment();
    }

    public static CommentRequest Request(string content)
    {
        return CommentRequest(content);
    }

    public static CommentResponse Response()
    {
        return Response("Great post!");
    }

    public static CommentResponse Response(string content)
    {
        return CommentResponse(content);
    }

    private static Comment EntityComment()
    {
        return new Comment
        {
            Id = 1L,
            Content = "Great post!",
            User = UserFactory.User(),
            Post = PostFactory.Post(),
            CreatedAt = MockDate,
            UpdatedAt = MockDate
        };
    }

    private static CommentRequest CommentRequest(String content)
    {
        return new CommentRequest(content);
    }

    private static CommentResponse CommentResponse(string content)
    {
        return new CommentResponse(1L, content, MockDate, MockDate, 
            UserFactory.ProfileResponse(), PostFactory.Response());
    }
}