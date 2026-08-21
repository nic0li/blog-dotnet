using Blog.DTOs.Authentication;
using Blog.DTOs.User;
using Blog.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController(
    IAuthenticationService authenticationService,
    IUserService userService) : ControllerBase
{
    private readonly IAuthenticationService _authenticationService = authenticationService;
    
    private readonly IUserService _userService = userService;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticationResponse>> Login(AuthenticationRequest request)
    {
        var response = await _authenticationService.AuthenticateAsync(request);

        return Ok(response);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserResponse>> Register(UserCreateRequest request)
    {
        var response = await _userService.CreateAsync(request);

        return StatusCode(StatusCodes.Status201Created, response);
    }
}