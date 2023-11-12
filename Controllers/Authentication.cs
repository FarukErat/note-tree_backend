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
    public async Task<IActionResult> SignUpAsync(SignupRequest request)
    {
        await _authenticationService.SignUpAsync(request, HttpContext);
        return Ok("User created successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        await _authenticationService.LoginAsync(request, HttpContext);
        return Ok("User logged in successfully");
    }

    [HttpGet("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await _authenticationService.LogoutAsync(HttpContext);
        return Ok("User logged out successfully");
    }

    [HttpGet("secret")]
    public async Task<IActionResult> SecretAsync()
    {
        await _authenticationService.SecretAsync(HttpContext);
        return Ok("You accessed the secret page");
    }
}
