using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

namespace IntegrationTests.Mocks.Grpc.Core;

internal static class GrpcTestCalls
{
    public static AsyncUnaryCall<T> UnaryResult<T>(T result)
    {
        return new AsyncUnaryCall<T>(
            Task.FromResult(result),
            Task.FromResult(new Metadata()),
            () => new Status(StatusCode.OK, string.Empty),
            () => [],
            () => { });
    }

    public static AsyncUnaryCall<T> UnaryEmpty<T>() where T : class, new() => UnaryResult(new T());

    public static AsyncUnaryCall<Empty> UnaryEmpty() => UnaryResult(new Empty());

    public static AsyncUnaryCall<T> UnaryError<T>(StatusCode code, string message)
    {
        return new AsyncUnaryCall<T>(
            Task.FromException<T>(new RpcException(new Status(code, message))),
            Task.FromResult(new Metadata()),
            () => new Status(code, message),
            () => [],
            () => { });
    }
}
