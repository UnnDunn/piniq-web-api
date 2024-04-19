namespace Pinball.OpdbClient.Interfaces.Impl
{
    internal record OpdbResponse : IOpdbResponse
    {
        public string JsonResponse { get; set; } = null!;
        public OpdbResponseStatus Status { get; set; }
    }

    internal record OpdbResponse<T> : OpdbResponse
    {
        public required T Result { get; set; }
    }

    internal enum OpdbResponseStatus { Ok, JsonError, ApiError };
}
