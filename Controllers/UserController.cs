using Blog.DTOs.User;
using Blog.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpPut("{id:long}")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> Update(long id,UserUpdateRequest request)
    {
        var response = await _service.UpdateAsync(id, request);

        return Ok(response);
    }

    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<IActionResult> Delete(long id)
    {
        await _service.DeleteAsync(id);

        return NoContent();
    }

    [HttpGet("{id:long}")]
    [Authorize]
    public async Task<ActionResult<UserViewResponse>> GetById(long id)
    {
        var response = await _service.GetByIdAsync(id);

        return Ok(response);
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserViewResponse>>> GetAll()
    {
        var response = await _service.GetAllAsync();

        return Ok(response);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> GetMe()
    {
        var response = await _service.GetMeAsync();

        return Ok(response);
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> UpdateMe(UserUpdateRequest request)
    {
        var response = await _service.UpdateMeAsync(request);

        return Ok(response);
    }

    [HttpDelete("me")]
    [Authorize]
    public async Task<IActionResult> DeleteMe()
    {
        await _service.DeleteMeAsync();

        return NoContent();
    }
}