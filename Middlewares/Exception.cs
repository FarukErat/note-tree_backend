using System.Net;
using Serilog;
using Workout.Errors;

namespace Workout.Middlewares;

class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode;
        string? message;
        switch (exception)
        {
            case CustomException e:
                statusCode = e.StatusCode;
                message = e.Message;
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                message = "Internal server error";

                // Log exception
                Console.WriteLine(exception);
                Log.Error(exception.ToString());
                break;
        }

        var response = context.Response;
        if (!response.HasStarted)
        {
            response.StatusCode = (int)statusCode;
            response.ContentType = "text/plain";
            await response.WriteAsync(message);
        }
    }
}