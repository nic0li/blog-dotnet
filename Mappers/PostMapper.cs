using Blog.DTOs.Post;
using Blog.Entities;

namespace Blog.Mappers;

public static class PostMapper
{
    public static Post CreateEntity(PostCreateRequest request)
    {
        var post = new Post
        {
            Title = request.Title,
            Content = request.Content
        };

        return post;
    }

    public static void UpdateEntity(
        Post post,
        PostUpdateRequest request)
    {
        if (request.Title is not null)
        {
            post.Title = request.Title;
        }

        if (request.Content is not null)
        {
            post.Content = request.Content;
        }
    }

    public static PostResponse ToResponse(Post post)
    {
        return new PostResponse(
            post.Id,
            post.Title,
            post.Content,
            CategoryMapper.ToResponse(post.Category),
            UserMapper.ToResponse(post.User),
            DateTime.SpecifyKind(post.CreatedAt, DateTimeKind.Utc),
            DateTime.SpecifyKind(post.UpdatedAt, DateTimeKind.Utc));
    }

    public static PostViewResponse ToViewResponse(Post post)
    {
        return new PostViewResponse(
            post.Id,
            post.Title,
            post.Content,
            CategoryMapper.ToResponse(post.Category),
            UserMapper.ToViewResponse(post.User),
            CommentMapper.ToViewResponse(post.Comments),
            DateTime.SpecifyKind(post.CreatedAt, DateTimeKind.Utc),
            DateTime.SpecifyKind(post.UpdatedAt, DateTimeKind.Utc));
    }
}