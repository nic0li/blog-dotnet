namespace Blog.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;

    public long UserId { get; set; }

    public User User { get; set; } = null!;

    public long PostId { get; set; }

    public Post Post { get; set; } = null!;
}