namespace Pinball.OpdbClient.Interfaces;

public interface IOpdbResponse
{
    string JsonResponse { get; }
}

public interface IOpdbResponse<out T> : IOpdbResponse
{
    T Result { get; }
}