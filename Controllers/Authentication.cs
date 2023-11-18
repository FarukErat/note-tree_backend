using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Workout.Dtos.Requests;
using Workout.Dtos.Responses;
using Workout.Interfaces;

namespace Workout.Controllers;

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

    [HttpGet("secret")]
    public async Task<IActionResult> SecretAsync()
    {
        ErrorOr<Success> secretResult = await _authenticationService.SecretAsync(HttpContext);

        return secretResult.Match(
            result => Ok(new { message = "You have access to the secret page!" }),
            errors => Problem(errors)
        );
    }
}
