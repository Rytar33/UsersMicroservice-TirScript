using FluentValidation;
using System.Net;
using TestUsers.Services.Dtos;
using TestUsers.Services.Exceptions;

namespace TestUsers.WebApi.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException validationException)
        {
            await HandleValidationExceptionAsync(context, validationException);
        }
        catch (ExceptionWithStatusCode ex)
        {
            await HandleExceptionWithStatusCodeAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var errors = exception.Errors;
        var errorResponse = new
        {
            StatusCode = context.Response.StatusCode,
            Errors = errors.Select(e => new
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage
            })
        };

        return context.Response.WriteAsJsonAsync(errorResponse);
    }

    private Task HandleExceptionWithStatusCodeAsync(HttpContext context, ExceptionWithStatusCode exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)exception.StatusCode;

        return context.Response.WriteAsJsonAsync(new BaseResponse(false, exception.Message));
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; ;

        return context.Response.WriteAsJsonAsync(new BaseResponse(false, "Произошла серверная ошибка, повторите попытку позже."));
    }
}