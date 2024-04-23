using System;

namespace Pinball.Api.Services.Entities.Exceptions;

public class OpdbException : Exception
{
    public OpdbException()
    {
    }

    public OpdbException(string? message) : base(message)
    {
    }

    public OpdbException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}