using System.Net;

namespace TestUsers.Services.Exceptions;

public class ExceptionWithStatusCode(string message, HttpStatusCode statusCode) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
