using Blog.DTOs.Authentication;
using Blog.DTOs.User;
using Blog.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    
    private readonly IUserService _userService;

    public AuthenticationController(
        IAuthenticationService authenticationService,
        IUserService userService)
    {
        _authenticationService = authenticationService;
        _userService = userService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request)
    {
        var response = await _authenticationService
            .AuthenticateAsync(request);

        return Ok(response);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserResponse>> Create(UserCreateRequest request)
    {
        var response = await _userService.CreateAsync(request);

        return StatusCode(StatusCodes.Status201Created, response);
    }
}