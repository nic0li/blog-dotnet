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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserViewResponse>>> GetAll()
    {
        var response = await _service.GetAllAsync();

        return Ok(response);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<UserViewResponse>> GetById(long id)
    {
        var response = await _service.GetByIdAsync(id);

        return Ok(response);
    }

    [HttpPatch("{id:long}")]
    public async Task<ActionResult<UserResponse>> Update(long id, UserUpdateRequest request)
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

    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetMe()
    {
        var response = await _service.GetMeAsync();

        return Ok(response);
    }

    [HttpPatch("me")]
    public async Task<ActionResult<UserResponse>> UpdateMe(UserUpdateRequest request)
    {
        var response = await _service.UpdateMeAsync(request);

        return Ok(response);
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe()
    {
        await _service.DeleteMeAsync();

        return NoContent();
    }

    [HttpPatch("me/password")]
    public async Task<IActionResult> UpdatePassword(UserPasswordUpdateRequest request)
    {
        await _service.UpdatePasswordAsync(request);

        return NoContent();
    }

    [HttpPatch("{id}/role")]
    public async Task<ActionResult<UserResponse>> ToggleRole(long id)
    {
        var response = await _service.ToggleRoleAsync(id);

        return Ok(response);
    }

    [HttpGet("{id:long}/posts")]
    public async Task<ActionResult<IEnumerable<PostViewResponse>>>
    GetPostsByUser(long id)
    {
        var response = await _postService.GetByUserAsync(id);

        return Ok(response);
    }

    [HttpGet("me/posts")]
    public async Task<ActionResult<IEnumerable<PostViewResponse>>>
    GetMyPosts()
    {
        var response = await _postService.GetByAuthenticatedUserAsync();

        return Ok(response);
    }
}