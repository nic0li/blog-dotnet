using System.ComponentModel.DataAnnotations;

namespace Blog.DTOs.Comment;

public record CommentRequest(
    [Required]
    string Content
);