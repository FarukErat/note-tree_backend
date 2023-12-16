using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Workout.Controllers;
using Workout.Authentication.Dtos.Requests;
using Workout.Authentication.Dtos.Responses;
using Workout.Authentication.Interfaces;
using WorkoutProject.Helpers;

namespace Workout.Authentication.Controllers;

public class Authentication : ApiController
{
    private readonly IAuthenticationService _authenticationService;

    public Authentication(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUpAsync([FromBody] SignupRequest request)
    {
        ErrorOr<SignupResponse> signupResult = await _authenticationService.SignUpAsync(request, HttpContext);

        return signupResult.Match(
            result => Ok(result),
            errors => Problem(errors)
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        ErrorOr<LoginResponse> loginResult = await _authenticationService.LoginAsync(request, HttpContext);

        return loginResult.Match(
            result => Ok(result),
            errors => Problem(errors)
        );
    }

    [HttpGet("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        ErrorOr<Success> logoutResult = await _authenticationService.LogoutAsync(HttpContext);

        return logoutResult.Match(
            result => Ok(new { message = "You have been logged out!" }),
            errors => Problem(errors)
        );
    }

    [SessionAuth]
    [HttpGet("secret")]
    public IActionResult SecretAsync()
    {
        return Ok(new { message = "You have access to the secret page!" });
    }
}