using ErrorOr;
using Workout.Dtos.Requests;
using Workout.Dtos.Responses;

namespace Workout.Interfaces;

public interface IAuthenticationService
{
    Task<ErrorOr<SignupResponse>> SignUpAsync(SignupRequest request, HttpContext httpContext);
    Task<ErrorOr<LoginResponse>> LoginAsync(LoginRequest request, HttpContext httpContext);
    Task<ErrorOr<Success>> LogoutAsync(HttpContext httpContext);
    Task<ErrorOr<Success>> SecretAsync(HttpContext httpContext);
}