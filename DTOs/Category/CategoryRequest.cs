using System.ComponentModel.DataAnnotations;

namespace Blog.DTOs.Category;

public record CategoryRequest(
    [Required]
    string Name
);