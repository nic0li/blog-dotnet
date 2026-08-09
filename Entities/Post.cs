namespace Blog.Entities;

public class Post : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public long CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public long UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}