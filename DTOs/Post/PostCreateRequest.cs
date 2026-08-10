using System.ComponentModel.DataAnnotations;

namespace Blog.DTOs.Post;

public record PostCreateRequest(
    [Required]
    string Title,

    [Required]
    string Content,

    [Required]
    long CategoryId
);