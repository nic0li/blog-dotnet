using Blog.DTOs.Comment;
using Blog.Entities;

namespace Blog.Tests.Factory;

public static class CommentFactory
{
    public static readonly DateTime MockDate =
        new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static Comment Comment()
    {
        return Comment("Great post!");
    }

    public static Comment UpdatedComment()
    {
        return Comment("Updated comment!");
    }

    public static CommentRequest Request()
    {
        return new CommentRequest("Great post!");
    }

    public static CommentRequest UpdateRequest()
    {
        return new CommentRequest("Updated comment!");
    }

    public static CommentResponse Response()
    {
        return Response("Great post!");
    }

    public static CommentResponse UpdatedResponse()
    {
        return Response("Updated comment!");
    }

    private static Comment Comment(string content)
    {
        return new Comment
        {
            Id = 1L,
            Content = content,
            User = UserFactory.User(),
            Post = PostFactory.Post(),
            CreatedAt = MockDate,
            UpdatedAt = MockDate
        };
    }

    private static CommentResponse Response(string content)
    {
        return new CommentResponse(1L,
            content,
            MockDate,
            MockDate,
            UserFactory.ProfileResponse(),
            PostFactory.Response());
    }
}