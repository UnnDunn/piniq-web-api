namespace Pinball.OpdbClient.Interfaces.Impl
{
    internal class OpdbResponse : IOpdbResponse
    {
        public string JsonResponse { get; set; } = null!;
        public OpdbResponseStatus Status { get; set; }
    }

    internal enum OpdbResponseStatus { Ok, JsonError, ApiError };
}
