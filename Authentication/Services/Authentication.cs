using ErrorOr;
using NoteTree.Constants;
using NoteTree.Authentication.Dtos.Requests;
using NoteTree.Authentication.Dtos.Responses;
using NoteTree.Authentication.Errors;
using NoteTree.Authentication.Interfaces;
using NoteTree.Authentication.Models;
using NoteTree.Notes.Interfaces;
using NoteTree.Notes.Models;

namespace NoteTree.Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserAuthDataManager _userAuthDb;
    private readonly ICacheService _cacheService;
    private readonly INoteDataManager _noteDataManager;
    private readonly string _hashAlgorithm;
    private readonly TimeSpan _sessionExpiry;

    public AuthenticationService(ConfigProvider configProvider, IUserAuthDataManager DBService, ICacheService cacheService, INoteDataManager noteDataManager)
    {
        _userAuthDb = DBService;
        _cacheService = cacheService;
        _hashAlgorithm = configProvider.HashAlgorithm;
        _sessionExpiry = configProvider.SessionExpiry;
        _noteDataManager = noteDataManager;
    }

    public async Task<ErrorOr<RegisterResponse>> RegisterAsync(RegisterRequest request, HttpContext httpContext)
    {
        // look up database for existing user
        User? existingUser = await _userAuthDb.FindByUsernameAsync(request.UserName);
        if (existingUser != null)
        {
            return Errors.Authentication.UserNameTaken;
        }

        // save new user to database
        IPasswordManager hasher = HasherFactory.GetHasherByAlgorithm(_hashAlgorithm);
        User user = new()
        {
            UserName = request.UserName,
            PasswordHash = hasher.HashPassword(request.Password),
            PasswordAlgo = _hashAlgorithm,
            Role = Roles.NewUser,
            NoteRecordId = await _noteDataManager.SaveNotesOfUser(new NoteRecord() { Notes = new List<Note>() })
        };
        await _userAuthDb.CreateUserAsync(user);

        // save user to cache and set cookie
        await LogUserIn(user, httpContext);

        // return response
        return new RegisterResponse
        {
            Id = user.Id!,
            UserName = user.UserName,
            Role = user.Role
        };
    }

    public async Task<ErrorOr<LoginResponse>> LoginAsync(LoginRequest request, HttpContext httpContext)
    {
        // look up database for existing user
        User? existingUser = await _userAuthDb.FindByUsernameAsync(request.UserName);
        if (existingUser == null)
        {
            return Errors.Authentication.UserNotFound;
        }

        // verify password
        IPasswordManager hasher = HasherFactory.GetHasherByAlgorithm(existingUser.PasswordAlgo);
        bool isPasswordCorrect = hasher.VerifyPassword(request.Password, existingUser.PasswordHash);
        if (!isPasswordCorrect)
        {
            return Errors.Authentication.IncorrectPassword;
        }

        // update password hash if algorithm has changed
        if (existingUser.PasswordAlgo != _hashAlgorithm)
        {
            hasher = HasherFactory.GetHasherByAlgorithm(_hashAlgorithm);
            existingUser.PasswordHash = hasher.HashPassword(request.Password);
            existingUser.PasswordAlgo = _hashAlgorithm;
            await _userAuthDb.UpdatePasswordAsync(existingUser.UserName, existingUser.PasswordHash, existingUser.PasswordAlgo);
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
            return Errors.Authentication.NotLoggedIn;
        }

        // delete session from cache
        await _cacheService.DeleteSessionByIdAsync(sessionId);

        // delete cookie
        httpContext.Response.Cookies.Delete(Cookies.SessionId);

        return Result.Success;
    }

    // TODO: prevent creating multiple sessions for same user when cookie is not set
    private async Task LogUserIn(User user, HttpContext httpContext)
    {
        string? sessionId = httpContext.Request.Cookies[Cookies.SessionId];
        Session? session = null; // to suppress use of unassigned variable warning
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