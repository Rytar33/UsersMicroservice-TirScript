using System.Net;
using System.Net.WebSockets;

namespace TestUsers.Services.Exceptions;

public class ExceptionWithStatusCode(string message, HttpStatusCode statusCode) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;

    public WebSocketCloseStatus State { get; }
}
