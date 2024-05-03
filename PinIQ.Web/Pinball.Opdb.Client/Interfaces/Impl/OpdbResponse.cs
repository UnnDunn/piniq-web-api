namespace Pinball.OpdbClient.Interfaces.Impl;

internal record OpdbResponse : IOpdbResponse
{
    public OpdbResponseStatus Status { get; set; }
    public string JsonResponse { get; set; } = null!;
}

internal record OpdbResponse<T> : OpdbResponse
{
    public required T Result { get; set; }
}

internal enum OpdbResponseStatus
{
    Ok,
    JsonError,
    ApiError
}