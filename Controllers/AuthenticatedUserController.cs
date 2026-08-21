using Blog.DTOs.Post;
using Blog.DTOs.User;
using Blog.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

[ApiController]
[Route("users/me")]
[Authorize]
public class AuthenticatedUserController(
    IUserService service,
    IPostService postService) : ControllerBase
{
    private readonly IUserService _service = service;

    private readonly IPostService _postService = postService;

    [HttpGet]
    public async Task<ActionResult<UserResponse>> GetAuthenticated()
    {
        var response = await _service.GetAuthenticatedAsync();

        return Ok(response);
    }

    [HttpPatch]
    public async Task<ActionResult<UserResponse>> UpdateAuthenticated(UserUpdateRequest request)
    {
        var response = await _service.UpdateAuthenticatedAsync(request);

        return Ok(response);
    }

    [HttpPatch("password")]
    public async Task<IActionResult> UpdateAuthenticatedPassword(UserPasswordUpdateRequest request)
    {
        await _service.UpdateAuthenticatedPasswordAsync(request);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAuthenticated()
    {
        await _service.DeleteAuthenticatedAsync();

        return NoContent();
    }

    [HttpGet("posts")]
    public async Task<ActionResult<IEnumerable<PostResponse>>> GetAuthenticatedUserPosts()
    {
        var response = await _postService.GetByAuthenticatedUserAsync();

        return Ok(response);
    }
}