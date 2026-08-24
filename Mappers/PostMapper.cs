using Blog.DTOs.Comment;
using Blog.DTOs.Post;
using Blog.Entities;

namespace Blog.Mappers;

public static class PostMapper
{
    public static Post CreateEntity(PostRequest request)
    {
        var post = new Post
        {
            Title = request.Title!,
            Content = request.Content!
        };
        return post;
    }

    public static void UpdateEntity(Post post, PostRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            post.Title = request.Title;
        }

        if (!string.IsNullOrWhiteSpace(request.Content))
        {
            post.Content = request.Content;
        }
    }

    public static PostResponse ToResponse(Post post)
    {
        return ToResponse(post,
            [.. CommentMapper.ToListResponse(post.Comments)]);
    }

    public static PostResponse ToResponseWithoutComments(Post post)
    {
        return ToResponse(post, []);
    }

    private static PostResponse ToResponse(Post post, List<CommentResponse>? comments)
    {
        return new PostResponse(
            post.Id,
            post.Title,
            post.Content,
            DateTime.SpecifyKind(post.CreatedAt, DateTimeKind.Utc),
            DateTime.SpecifyKind(post.UpdatedAt, DateTimeKind.Utc),
            CategoryMapper.ToResponse(post.Category),
            UserMapper.ToProfileResponse(post.User),
            comments);
    }
}