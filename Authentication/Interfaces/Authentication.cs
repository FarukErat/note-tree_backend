using ErrorOr;
using Workout.Authentication.Dtos.Requests;
using Workout.Authentication.Dtos.Responses;
using Workout.Authentication.Models;

namespace Workout.Authentication.Interfaces;

public interface IAuthenticationService
{
    Task<ErrorOr<SignupResponse>> SignUpAsync(SignupRequest request, HttpContext httpContext);
    Task<ErrorOr<LoginResponse>> LoginAsync(LoginRequest request, HttpContext httpContext);
    Task<ErrorOr<Success>> LogoutAsync(HttpContext httpContext);
}