using Blog.DTOs.Comment;
using Blog.Entities;

namespace Blog.Mappers;

public static class CommentMapper
{
    public static Comment CreateEntity(CommentRequest request)
    {
        var comment = new Comment
        {
            Content = request.Content
        };
        return comment;
    }

    public static void UpdateEntity(Comment comment, CommentRequest request)
    {
        if (request.Content is not null)
        {
            comment.Content = request.Content;
        }
    }

    public static CommentResponse ToResponse(Comment comment)
    {
        return ToResponse(comment, true);
    }

    public static CommentResponse ToResponseWithoutPost(Comment comment)
    {
        return ToResponse(comment, false);
    }

    public static List<CommentResponse> ToListResponseWithoutPost(IEnumerable<Comment> comments)
    {
        return [.. comments.Select(ToResponseWithoutPost)];
    }

    private static CommentResponse ToResponse(Comment comment, bool includePost)
    {
        return new CommentResponse(
            comment.Id,
            comment.Content,
            DateTime.SpecifyKind(comment.CreatedAt, DateTimeKind.Utc),
            DateTime.SpecifyKind(comment.UpdatedAt, DateTimeKind.Utc),
            UserMapper.ToProfileResponse(comment.User),
            includePost
                ? PostMapper.ToResponseWithoutComments(comment.Post)
                : null);
    }
}