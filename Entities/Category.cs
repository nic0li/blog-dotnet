namespace Blog.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Post> Posts { get; set; } = new List<Post>();
}