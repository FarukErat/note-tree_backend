using System.Net;
// using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NoteTree.Authentication.Interfaces;
using NoteTree.Authentication.Models;
using NoteTree.Constants;

namespace NoteTree.Helpers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SessionAuthAttribute : TypeFilterAttribute
{
    public SessionAuthAttribute() : base(typeof(SessionAuthFilter))
    {
    }
}

public class SessionAuthFilter : IAsyncAuthorizationFilter
{
    private readonly ICacheService _cacheService;

    public SessionAuthFilter(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        ProblemDetails problemDetails = new()
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
            Title = "Authentication credentials were missing or incorrect.",
            Status = (int)HttpStatusCode.Unauthorized,
            Instance = context.HttpContext.Request.Path
        };

        // get session id from cookie
        string? sessionId = context.HttpContext.Request.Cookies[Cookies.SessionId];
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            context.Result = new UnauthorizedObjectResult(problemDetails);
            return;
        }

        // get session from cache
        Session? session = await _cacheService.GetSessionByIdAsync(sessionId);
        if (session == null)
        {
            context.Result = new UnauthorizedObjectResult(problemDetails);
            return;
        }

        // store session in context
        context.HttpContext.Items["Session"] = session;
        // context.HttpContext.User = new ClaimsPrincipal(
        //     new ClaimsIdentity(
        //         new Claim[] {
        //             new(ClaimTypes.Name, session.UserId),
        //             new(ClaimTypes.Role, session.Role),
        //             new("NoteRecordId", session.NoteRecordId ?? string.Empty)
        //         }
        //     )
        // );
    }
}
