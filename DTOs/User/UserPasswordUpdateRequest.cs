using System.ComponentModel.DataAnnotations;

namespace Blog.DTOs.User;

public record UserPasswordUpdateRequest(
    [Required(ErrorMessage = "Current password is required")]
    string CurrentPassword,

    [Required(ErrorMessage = "New password is required")]
    [MinLength(3, ErrorMessage = "Password must be at least 3 characters long")]
    string NewPassword
);