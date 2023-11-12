using System.Net;
using Workout.Constants;
using Workout.Dtos.Requests;
using Workout.Errors;
using Workout.Interfaces;
using Workout.Models;

namespace Workout.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IDBService _DBService;
    private readonly ICacheService _cacheService;
    private readonly string _hashAlgorithm;

    public AuthenticationService(ConfigProvider configProvider, IDBService DBService, ICacheService cacheService)
    {
        _DBService = DBService;
        _cacheService = cacheService;
        _hashAlgorithm = configProvider.HashAlgorithm;
    }

    public async Task SignUpAsync(SignupRequest request, HttpContext httpContext)
    {
        // look up database for existing user
        var existingUser = await _DBService.FindByUsernameAsync(request.UserName);
        if (existingUser != null)
        {
            throw new CustomException(ErrorMessages.UsernameTaken, HttpStatusCode.Conflict);
        }

        // save new user to database
        var hasher = HasherFactory.GetHasherByAlgorithm(_hashAlgorithm);
        var user = new AppUser
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

        // save user to cache
        await _cacheService.SaveUserAsync(user, httpContext);
    }

    public async Task LoginAsync(LoginRequest request, HttpContext httpContext)
    {
        // look up database for existing user
        var existingUser = await _DBService.FindByUsernameAsync(request.UserName);
        if (existingUser == null)
        {
            throw new CustomException(ErrorMessages.UserNotFound, HttpStatusCode.NotFound);
        }

        // verify password
        var hasher = HasherFactory.GetHasherByAlgorithm(existingUser.Password!.Algorithm);
        var isPasswordCorrect = hasher.VerifyPassword(request.Password, existingUser.Password.Hash);
        if (!isPasswordCorrect)
        {
            throw new CustomException(ErrorMessages.WrongPassword, HttpStatusCode.Unauthorized);
        }

        // update password hash if algorithm has changed
        if (existingUser.Password.Algorithm != _hashAlgorithm)
        {
            hasher = HasherFactory.GetHasherByAlgorithm(_hashAlgorithm);
            existingUser.Password.Hash = hasher.HashPassword(request.Password);
            existingUser.Password.Algorithm = _hashAlgorithm;
            await _DBService.UpdatePasswordAsync(existingUser.UserName, existingUser.Password);
        }

        // save user to cache
        await _cacheService.SaveUserAsync(existingUser, httpContext);
    }

    public async Task LogoutAsync(HttpContext httpContext)
    {
        // get session id from cookie
        var sessionId = httpContext.Request.Cookies[Cookies.SessionId]
            ?? throw new CustomException(ErrorMessages.NotLoggedIn, HttpStatusCode.Unauthorized);

        // delete session from cache
        await _cacheService.DeleteSessionByIdAsync(sessionId);

        // delete cookie
        httpContext.Response.Cookies.Delete(Cookies.SessionId);
    }

    public async Task SecretAsync(HttpContext httpContext)
    {
        // get session id from cookie
        var sessionId = httpContext.Request.Cookies[Cookies.SessionId]
            ?? throw new CustomException(ErrorMessages.NotLoggedIn, HttpStatusCode.Unauthorized);

        // get session from cache
        var session = await _cacheService.GetSessionByIdAsync(sessionId)
            ?? throw new CustomException(ErrorMessages.NotLoggedIn, HttpStatusCode.Unauthorized);

        // update session last seen
        session.LastSeen = DateTime.UtcNow;
        await _cacheService.UpdateSessionByIdAsync(sessionId, session);
    }
}