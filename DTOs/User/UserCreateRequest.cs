using System.ComponentModel.DataAnnotations;

namespace Blog.DTOs.User;

public record UserCreateRequest(
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email")]
    string Email,

    [Required(ErrorMessage = "Password is required")]
    [MinLength(3, ErrorMessage = "Password must be at least 3 characters long")]
    string Password,

    string? Name
);