using Common.Constants;

using Grpc.Core;

using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.Logging;

using Pl.Api.Http.Dtos.Core;

namespace Pl.Api.Http.Middlewares;

public sealed class ExceptionsMiddleware(RequestDelegate next, ILogger<ExceptionsMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (RpcException ex)
        {
            logger.LogWarning(ex, "gRPC ошибка: {StatusCode}, Detail: {Detail}", ex.StatusCode, ex.Status.Detail);

            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = MapGrpcStatusToHttp(ex.StatusCode);
                var response = RestApiResponseBuilder<object>.Fail(ex.Status.Detail, "gRPC ошибка при взаимодействии с внутренним сервисом.");
                await context.Response.WriteAsJsonAsync(response);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Необработанное исключение");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var errorResponse = RestApiResponseBuilder<object>.Fail("Внутренняя ошибка сервера", property: ErrorMessagesConstants.InternalServerError);
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }

    private static int MapGrpcStatusToHttp(StatusCode statusCode)
    {
        return statusCode switch
        {
            StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
            StatusCode.NotFound => StatusCodes.Status404NotFound,
            StatusCode.Unauthenticated => StatusCodes.Status401Unauthorized,
            StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
            StatusCode.AlreadyExists => StatusCodes.Status409Conflict,
            StatusCode.FailedPrecondition => StatusCodes.Status412PreconditionFailed,
            StatusCode.ResourceExhausted => StatusCodes.Status429TooManyRequests,
            StatusCode.Unavailable => StatusCodes.Status503ServiceUnavailable,
            StatusCode.Unimplemented => StatusCodes.Status501NotImplemented,
            StatusCode.DeadlineExceeded => StatusCodes.Status504GatewayTimeout,
            StatusCode.Internal => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}

