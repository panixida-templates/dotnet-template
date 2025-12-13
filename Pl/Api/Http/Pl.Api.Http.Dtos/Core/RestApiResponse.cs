namespace Pl.Api.Http.Dtos.Core;

public sealed record RestApiResponse<T>
{
    public T? Payload { get; set; }
    public Failure? Failure { get; set; }

    public bool IsSuccess => Failure == null;

    private RestApiResponse(Failure failure)
    {
        Failure = failure;
    }

    private RestApiResponse(T payload)
    {
        Payload = payload;
    }

    public static RestApiResponse<T> Fail(Failure failure)
    {
        return new RestApiResponse<T>(failure);
    }

    public static RestApiResponse<T> Success(T payload)
    {
        return new RestApiResponse<T>(payload);
    }
}
