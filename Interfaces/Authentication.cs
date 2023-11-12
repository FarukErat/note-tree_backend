using Workout.Dtos.Requests;

namespace Workout.Interfaces;

public interface IAuthenticationService
{
    Task SignUpAsync(SignupRequest request, HttpContext httpContext);
    Task LoginAsync(LoginRequest request, HttpContext httpContext);
    Task LogoutAsync(HttpContext httpContext);
    Task SecretAsync(HttpContext httpContext);
}