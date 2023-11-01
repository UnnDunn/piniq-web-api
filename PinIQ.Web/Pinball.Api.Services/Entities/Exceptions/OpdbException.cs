using System;
using System.Runtime.Serialization;

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

    protected OpdbException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}