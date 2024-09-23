﻿namespace Models.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string? message, Exception? innerException) : base(message, innerException) {}

    public NotFoundException(string? message) : base(message) {}

    public NotFoundException() {}
}