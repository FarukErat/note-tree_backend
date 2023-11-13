using Microsoft.AspNetCore.Mvc;
using Workout.Dtos.Requests;
using Workout.Interfaces;

namespace Workout.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Authentication : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public Authentication(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUpAsync([FromBody] SignupRequest request)
    {
        await _authenticationService.SignUpAsync(request, HttpContext);
        return Ok(new { message = "User created successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        await _authenticationService.LoginAsync(request, HttpContext);
        return Ok(new { message = "User logged in successfully" });
    }

    [HttpGet("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await _authenticationService.LogoutAsync(HttpContext);
        return Ok(new { message = "User logged out successfully" });
    }

    [HttpGet("secret")]
    public async Task<IActionResult> SecretAsync()
    {
        await _authenticationService.SecretAsync(HttpContext);
        return Ok(new { message = "You have accessed the secret page" });
    }
}
