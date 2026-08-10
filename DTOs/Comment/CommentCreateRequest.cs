using System.ComponentModel.DataAnnotations;

namespace Blog.DTOs.Comment;

public record CommentCreateRequest(
    [Required]
    string Content,

    [Required]
    long PostId
);