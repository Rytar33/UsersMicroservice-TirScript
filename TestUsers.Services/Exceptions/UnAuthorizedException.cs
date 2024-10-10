using System.Net;

namespace TestUsers.Services.Exceptions;

public class UnAuthorizedException(string message) : ExceptionWithStatusCode(message, HttpStatusCode.Unauthorized);