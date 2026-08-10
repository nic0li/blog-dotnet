namespace Blog.DTOs.Post;

public record PostFiltersRequest(
    string? Title,
    string? Category
)
{
    public bool HasTitle =>
        !string.IsNullOrWhiteSpace(Title);

    public bool HasCategory =>
        !string.IsNullOrWhiteSpace(Category);

    public bool IsEmpty =>
        !HasTitle && !HasCategory;
}