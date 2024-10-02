using System.Net;

namespace TestUsers.Services.Exceptions;

public class ConcidedException(string message) : ExceptionWithStatusCode(message, HttpStatusCode.BadRequest);