using Blog.DTOs.Comment;
using Blog.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

[ApiController]
[Route("comments")]
[Authorize]
public class CommentController(ICommentService service) : ControllerBase
{
    private readonly ICommentService _service = service;

    [HttpPatch("{id:long}")]
    public async Task<ActionResult<CommentResponse>> Update(long id, CommentRequest request)
    {
        var response = await _service.UpdateAsync(id, request);

        return Ok(response);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }
}