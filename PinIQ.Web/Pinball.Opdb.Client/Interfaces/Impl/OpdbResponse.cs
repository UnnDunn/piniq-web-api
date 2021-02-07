namespace Pinball.OpdbClient.Interfaces.Impl
{
    internal class OpdbResponse : IOpdbResponse
    {
        public string JsonResponse { get; set; }
        public OpdbResponseStatus Status { get; set; }
    }

    internal enum OpdbResponseStatus { Ok, JsonError, ApiError };
}
