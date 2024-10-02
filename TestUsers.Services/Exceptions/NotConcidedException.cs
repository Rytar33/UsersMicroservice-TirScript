using System.Net;

namespace TestUsers.Services.Exceptions;

public class NotConcidedException(string message) : ExceptionWithStatusCode(message, HttpStatusCode.BadRequest);