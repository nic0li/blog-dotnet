using System.ComponentModel.DataAnnotations;

namespace Blog.DTOs.User;

public record UserUpdateRequest(
    [EmailAddress(ErrorMessage = "Invalid email")]
    string? Email,

    string? Name,

    string? Photo,

    string? Bio
);