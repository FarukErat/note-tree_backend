using ErrorOr;
using NoteTree.Authentication.Dtos.Requests;
using NoteTree.Authentication.Dtos.Responses;
using NoteTree.Authentication.Models;

namespace NoteTree.Authentication.Interfaces;

public interface IAuthenticationService
{
    Task<ErrorOr<SignupResponse>> SignUpAsync(SignupRequest request, HttpContext httpContext);
    Task<ErrorOr<LoginResponse>> LoginAsync(LoginRequest request, HttpContext httpContext);
    Task<ErrorOr<Success>> LogoutAsync(HttpContext httpContext);
}