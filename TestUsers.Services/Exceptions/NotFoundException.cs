using System.Net;

namespace TestUsers.Services.Exceptions;

public class NotFoundException(string message) : ExceptionWithStatusCode(message, HttpStatusCode.NotFound);