using System.Text.Json.Serialization;

namespace Pl.Api.Http.Dtos.Core;

public sealed record Failure
{
    public Dictionary<string, string> Errors { get; } = [];

    public Failure() { }

    private Failure(string? error, string property = "")
    {
        if (!string.IsNullOrEmpty(error))
        {
            Errors[property] = error;
        }
    }

    public static Failure Create(string? error, string property = "")
    {
        return new Failure(error, property);
    }

    public Failure AddError(string error, string property = "")
    {
        Errors[property] = error;
        return this;
    }
}
