using Core.Exceptions;
using FluentValidation;
using Newtonsoft.Json;
using System.Net;

namespace API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, ex.Message);

        (HttpStatusCode Code, string Message) = ex switch
        {
            AlreadyExistsException => (HttpStatusCode.Conflict, "Resource already exists"),
            BadRequestException => (HttpStatusCode.BadRequest, "Bad request"),
            NotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            _ => (HttpStatusCode.InternalServerError, "Something went wrong :(")
        };

        if (ex.Message != string.Empty)
        {
            Message = ex.Message;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)Code;

        var result = JsonConvert.SerializeObject(new
        {
            context.Response.StatusCode,
            Message,
        });
        return context.Response.WriteAsync(result);
    }
}
