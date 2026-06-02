using Microsoft.AspNetCore.Mvc;
using ReqFlow.Application;

namespace ReqFlow.Api;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var (status, title) = exception switch
            {
                ValidationException => (StatusCodes.Status400BadRequest, "Validation failed"),
                NotFoundException => (StatusCodes.Status404NotFound, "Request not found"),
                ConflictException => (StatusCodes.Status409Conflict, "Request conflict"),
                ForbiddenException => (StatusCodes.Status403Forbidden, "Action forbidden"),
                _ => (StatusCodes.Status500InternalServerError, "Unexpected server error")
            };

            if (status == StatusCodes.Status500InternalServerError)
            {
                logger.LogError(exception, "Unhandled request error");
            }

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = status == StatusCodes.Status500InternalServerError ? "An unexpected error occurred." : exception.Message,
                Instance = context.Request.Path
            });
        }
    }
}
