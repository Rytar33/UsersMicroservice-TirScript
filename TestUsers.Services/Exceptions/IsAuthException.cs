using System.Net;

namespace TestUsers.Services.Exceptions;

public class IsAuthException(string message) : ExceptionWithStatusCode(message, HttpStatusCode.BadRequest);
