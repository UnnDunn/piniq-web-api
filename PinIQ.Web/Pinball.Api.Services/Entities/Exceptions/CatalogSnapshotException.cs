﻿using System;
using System.Runtime.Serialization;

namespace Pinball.Api.Services.Entities.Exceptions;

public class CatalogSnapshotException : Exception
{
    public int SnapshotId { get; private set; }

    public CatalogSnapshotException()
    {
    }

    public CatalogSnapshotException(string message) : base(message)
    {
    }

    public CatalogSnapshotException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected CatalogSnapshotException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public CatalogSnapshotException(int id)
    {
        SnapshotId = id;
    }

    public CatalogSnapshotException(int id, string message) : base (message)
    {
        SnapshotId = id;
    }

    public CatalogSnapshotException(int id, string message, Exception exception) : base (message, exception)
    {
        SnapshotId = id;
    }
    protected CatalogSnapshotException(int id, SerializationInfo info, StreamingContext context) : base(info, context)
    {
        SnapshotId = id;
    }
}