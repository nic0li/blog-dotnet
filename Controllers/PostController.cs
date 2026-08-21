using Blog.DTOs.Comment;
using Blog.DTOs.Post;
using Blog.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

[ApiController]
[Route("posts")]
[Authorize]
public class PostController(IPostService service, ICommentService commentService) : ControllerBase
{
    private readonly IPostService _service = service;
    private readonly ICommentService _commentService = commentService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostResponse>>> GetAll(
        [FromQuery] PostFiltersRequest filters)
    {
        var response = await _service.GetAllAsync(filters);

        return Ok(response);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PostResponse>> GetById(long id)
    {
        var response = await _service.GetByIdAsync(id);

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<PostResponse>> Create(PostRequest request)
    {
        var response = await _service.CreateAsync(request);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPatch("{id:long}")]
    public async Task<ActionResult<PostResponse>> Update(long id, PostRequest request)
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

    [HttpPost("{id:long}/comments")]
    public async Task<ActionResult<CommentResponse>> CreateComment(
        long id, CommentRequest request)
    {
        var response = await _commentService.CreateAsync(id, request);

        return StatusCode(StatusCodes.Status201Created, response);
    }
}