using FluentValidation;
using System.Net.WebSockets;
using System.Text.Json;
using TestUsers.Services.Exceptions;

namespace TestUsers.WebSocketApi.Middlewares;

public class WebSocketExceptionHandlingMiddleware(RequestDelegate next, ILogger<WebSocketExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            WebSocket? webSocket = null;
            try
            {
                webSocket = await context.WebSockets.AcceptWebSocketAsync();
            }
            catch (ValidationException validationException)
            {
                await HandleValidationExceptionAsync(webSocket, validationException);
            }
            catch (ExceptionWithStatusCode ex)
            {
                await HandleExceptionWithStatusCodeAsync(webSocket, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(webSocket, ex);
            }
        }
        else
        {
            await next(context); // передаем управление следующему middleware
        }
    }

    private Task HandleValidationExceptionAsync(WebSocket? webSocket, ValidationException exception)
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            var errorResponse = new
            {
                StatusCode = 400,
                Errors = exception.Errors.Select(e => new
                {
                    PropertyName = e.PropertyName,
                    ErrorMessage = e.ErrorMessage
                })
            };

            var message = JsonSerializer.Serialize(errorResponse);
            var bytes = System.Text.Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(bytes);

            return webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        return Task.CompletedTask;
    }

    private Task HandleExceptionWithStatusCodeAsync(WebSocket? webSocket, ExceptionWithStatusCode exception)
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            var errorResponse = new { StatusCode = (int)exception.StatusCode, Message = exception.Message };
            var message = JsonSerializer.Serialize(errorResponse);
            var bytes = System.Text.Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(bytes);

            return webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        return Task.CompletedTask;
    }

    private Task HandleExceptionAsync(WebSocket? webSocket, Exception ex)
    {
        logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);

        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            var errorResponse = new { StatusCode = 500, Message = "Произошла серверная ошибка, повторите попытку позже." };
            var message = JsonSerializer.Serialize(errorResponse);
            var bytes = System.Text.Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(bytes);

            return webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        return Task.CompletedTask;
    }
}