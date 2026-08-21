using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Blog.DTOs.Post;

public record PostRequest(
    string? Title,
    string? Content,
    long? CategoryId
)
{
    [BindNever]
    public bool HasTitle =>
        !string.IsNullOrWhiteSpace(Title);

    [BindNever]
    public bool HasContent =>
        !string.IsNullOrWhiteSpace(Content);

    [BindNever]
    public bool HasCategoryId =>
        CategoryId is not null;
};