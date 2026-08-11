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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentViewResponse>>> GetAll()
    {
        var response = await _service.GetAllAsync();

        return Ok(response);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CommentViewResponse>> GetById(long id)
    {
        var response = await _service.GetByIdAsync(id);

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<CommentResponse>> Create(CommentCreateRequest request)
    {
        var response = await _service.CreateAsync(request);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPatch("{id:long}")]
    public async Task<ActionResult<CommentResponse>> Update(long id, CommentUpdateRequest request)
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