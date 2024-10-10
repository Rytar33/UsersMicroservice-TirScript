using System.Net;

namespace TestUsers.Services.Exceptions;

public class ForbiddenException(string message) : ExceptionWithStatusCode(message, HttpStatusCode.Forbidden);