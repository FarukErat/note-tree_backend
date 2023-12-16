using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NoteTree.Filters;

public class ExceptionHandlerFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        Serilog.Log.Error(context.Exception.ToString());

        ProblemDetails problemDetails = new()
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "An unexpected error occurred while processing your request.",
            Status = (int)HttpStatusCode.InternalServerError,
            Instance = context.HttpContext.Request.Path
        };

        context.Result = new ObjectResult(problemDetails);

        context.ExceptionHandled = true;
    }
}