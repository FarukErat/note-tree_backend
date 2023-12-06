using System.Net;
using ErrorOr;
using Workout.Constants;
using Workout.Dtos.Requests;
using Workout.Dtos.Responses;
using Workout.Errors;
using Workout.Interfaces;
using Workout.Models;

namespace Workout.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IDBService _DBService;
    private readonly ICacheService _cacheService;
    private readonly string _hashAlgorithm;
    private readonly TimeSpan _sessionExpiry;

    public AuthenticationService(ConfigProvider configProvider, IDBService DBService, ICacheService cacheService)
    {
        _DBService = DBService;
        _cacheService = cacheService;
        _hashAlgorithm = configProvider.HashAlgorithm;
        _sessionExpiry = configProvider.SessionExpiry;
    }

    public async Task<ErrorOr<SignupResponse>> SignUpAsync(SignupRequest request, HttpContext httpContext)
    {
        // look up database for existing user
        User? existingUser = await _DBService.FindByUsernameAsync(request.UserName);
        if (existingUser != null)
        {
            return Authentication.UserNameTaken;
        }

        // save new user to database
        IPasswordManager hasher = HasherFactory.GetHasherByAlgorithm(_hashAlgorithm);
        User user = new()
        {
            UserName = request.UserName,
            Password = new Password
            {
                Algorithm = _hashAlgorithm,
                Hash = hasher.HashPassword(request.Password)
            },
            Role = Roles.NewUser
        };
        await _DBService.CreateUserAsync(user);

        // save user to cache and set cookie
        await LogUserIn(user, httpContext);

        // return response
        return new SignupResponse
        {
            Id = user.Id!,
            UserName = user.UserName,
            Role = user.Role
        };
    }

    public async Task<ErrorOr<LoginResponse>> LoginAsync(LoginRequest request, HttpContext httpContext)
    {
        // look up database for existing user
        User? existingUser = await _DBService.FindByUsernameAsync(request.UserName);
        if (existingUser == null)
        {
            return Authentication.UserNotFound;
        }

        // verify password
        IPasswordManager hasher = HasherFactory.GetHasherByAlgorithm(existingUser.Password!.Algorithm);
        bool isPasswordCorrect = hasher.VerifyPassword(request.Password, existingUser.Password.Hash);
        if (!isPasswordCorrect)
        {
            return Authentication.IncorrectPassword;
        }

        // update password hash if algorithm has changed
        if (existingUser.Password.Algorithm != _hashAlgorithm)
        {
            hasher = HasherFactory.GetHasherByAlgorithm(_hashAlgorithm);
            existingUser.Password.Hash = hasher.HashPassword(request.Password);
            existingUser.Password.Algorithm = _hashAlgorithm;
            await _DBService.UpdatePasswordAsync(existingUser.UserName, existingUser.Password);
        }

        // save user to cache and set cookie
        await LogUserIn(existingUser, httpContext);

        // return response
        return new LoginResponse
        {
            Id = existingUser.Id!,
            UserName = existingUser.UserName,
            Role = existingUser.Role
        };
    }

    public async Task<ErrorOr<Success>> LogoutAsync(HttpContext httpContext)
    {
        // get session id from cookie
        string? sessionId = httpContext.Request.Cookies[Cookies.SessionId];

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return Authentication.NotLoggedIn;
        }

        // delete session from cache
        await _cacheService.DeleteSessionByIdAsync(sessionId);

        // delete cookie
        httpContext.Response.Cookies.Delete(Cookies.SessionId);

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> SecretAsync(HttpContext httpContext)
    {
        // get session id from cookie
        string? sessionId = httpContext.Request.Cookies[Cookies.SessionId];

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return Authentication.NotLoggedIn;
        }

        // get session from cache
        Session? session = await _cacheService.GetSessionByIdAsync(sessionId);

        if (session == null)
        {
            return Authentication.NotLoggedIn;
        }

        // update session last seen
        session.LastSeen = DateTime.UtcNow;
        await _cacheService.UpdateSessionByIdAsync(sessionId, session);

        return Result.Success;
    }

    private async Task LogUserIn(User user, HttpContext httpContext)
    {
        string? sessionId = httpContext.Request.Cookies[Cookies.SessionId];
        Session? session = null;
        bool isSessionValid = false;

        if (!string.IsNullOrWhiteSpace(sessionId))
        {
            session = await _cacheService.GetSessionByIdAsync(sessionId);
            if (session != null)
            {
                isSessionValid = true;
            }
        }
        if (isSessionValid)
        {
            if (session!.UserId != user.Id)
            {
                await _cacheService.DeleteSessionByIdAsync(sessionId!);
            }
            else
            {
                session.LastSeen = DateTime.UtcNow;
                session.ExpireAt = DateTime.UtcNow.Add(_sessionExpiry);
                await _cacheService.UpdateSessionByIdAsync(sessionId!, session);
                return;
            }
        }

        // save user to cache
        sessionId = await _cacheService.SaveUserAsync(user, httpContext);

        // set cookie
        httpContext.Response.Cookies.Append(Cookies.SessionId, sessionId, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Secure = true,
            Expires = DateTime.UtcNow.Add(_sessionExpiry)
        });
    }
}