using System;

namespace Pinball.Api.Services.Entities.Exceptions;

public class CatalogSnapshotException : Exception
{
    public CatalogSnapshotException()
    {
    }

    public CatalogSnapshotException(string message) : base(message)
    {
    }

    public CatalogSnapshotException(string message, Exception innerException) : base(message, innerException)
    {
    }


    public CatalogSnapshotException(int id)
    {
        SnapshotId = id;
    }

    public CatalogSnapshotException(int id, string message) : base(message)
    {
        SnapshotId = id;
    }

    public CatalogSnapshotException(int id, string message, Exception exception) : base(message, exception)
    {
        SnapshotId = id;
    }

    public int SnapshotId { get; private set; }
}