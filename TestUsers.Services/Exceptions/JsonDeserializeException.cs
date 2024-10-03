using System.Net;

namespace TestUsers.Services.Exceptions;

public class JsonDeserializeException(string message, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError) : ExceptionWithStatusCode(message, httpStatusCode);