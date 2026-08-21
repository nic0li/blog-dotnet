using Blog.DTOs.Post;
using Blog.DTOs.User;
using Blog.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public class UserController(
    IUserService service, 
    IPostService postService) : ControllerBase
{
    private readonly IUserService _service = service;
    private readonly IPostService _postService = postService;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<UserProfileResponse>> GetById(long id)
    {
        var response = await _service.GetByIdAsync(id);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserProfileResponse>>> GetAll()
    {
        var response = await _service.GetAllAsync();

        return Ok(response);
    }

    [HttpPatch("{id}/role")]
    public async Task<ActionResult<UserResponse>> ToggleUserRole(long id)
    {
        var response = await _service.ToggleUserRoleAsync(id);

        return Ok(response);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteUser(long id)
    {
        await _service.DeleteUserAsync(id);

        return NoContent();
    }

    [HttpGet("{id:long}/posts")]
    public async Task<ActionResult<IEnumerable<PostResponse>>> GetPostsByUser(long id)
    {
        var response = await _postService.GetByUserAsync(id);

        return Ok(response);
    }
}