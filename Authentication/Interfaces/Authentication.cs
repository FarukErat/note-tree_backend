using ErrorOr;
using NoteTree.Authentication.Dtos.Requests;
using NoteTree.Authentication.Dtos.Responses;
using NoteTree.Authentication.Models;

namespace NoteTree.Authentication.Interfaces;

public interface IAuthenticationService
{
    Task<ErrorOr<RegisterResponse>> RegisterAsync(RegisterRequest request, HttpContext httpContext);
    Task<ErrorOr<LoginResponse>> LoginAsync(LoginRequest request, HttpContext httpContext);
    Task<ErrorOr<Success>> LogoutAsync(HttpContext httpContext);
}