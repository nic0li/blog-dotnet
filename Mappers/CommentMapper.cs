using Blog.DTOs.Comment;
using Blog.Entities;

namespace Blog.Mappers;

public static class CommentMapper
{
    public static Comment CreateEntity(CommentCreateRequest request)
    {
        var comment = new Comment
        {
            Content = request.Content
        };

        return comment;
    }

    public static void UpdateEntity(
        Comment comment,
        CommentUpdateRequest request)
    {
        if (request.Content is not null)
        {
            comment.Content = request.Content;
        }
    }

    public static CommentResponse ToResponse(Comment comment)
    {
        return new CommentResponse(
            comment.Id,
            comment.Content,
            UserMapper.ToResponse(comment.User),
            DateTime.SpecifyKind(comment.CreatedAt, DateTimeKind.Utc),
            DateTime.SpecifyKind(comment.UpdatedAt, DateTimeKind.Utc));
    }

    public static CommentViewResponse ToViewResponse(Comment comment)
    {
        return new CommentViewResponse(
            comment.Id,
            comment.Content,
            UserMapper.ToViewResponse(comment.User),
            DateTime.SpecifyKind(comment.CreatedAt, DateTimeKind.Utc),
            DateTime.SpecifyKind(comment.UpdatedAt, DateTimeKind.Utc));
    }

    public static CommentViewResponse[] ToViewResponse(
        IEnumerable<Comment> comments)
    {
        return [.. comments.Select(ToViewResponse)];
    }
}