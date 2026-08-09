using Blog.Enums;

namespace Blog.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string? Name { get; set; }

    public string? Photo { get; set; }

    public string? Bio { get; set; }

    public UserRole Role { get; set; }

    public ICollection<Post> Posts { get; set; } = new List<Post>();

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}