using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Blog.DTOs.Post;

public record PostFiltersRequest(
    string? Title,
    string? Category
)
{
    [BindNever]
    public bool HasTitle =>
        !string.IsNullOrWhiteSpace(Title);
    
    [BindNever]
    public bool HasCategory => 
        !string.IsNullOrWhiteSpace(Category);
}