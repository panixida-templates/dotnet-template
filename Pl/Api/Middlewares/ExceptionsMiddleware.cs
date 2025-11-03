using System;
using System.Net.Mime;
using System.Threading.Tasks;

using Api.Infrastructure.Core;
using Api.Infrastructure.Responses.Core;

using Common.Constants;
using Common.Exceptions;

using Grpc.Core;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Api.Middlewares;

public sealed class ExceptionsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionsMiddleware> _logger;

    public ExceptionsMiddleware(RequestDelegate next, ILogger<ExceptionsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (RpcException ex)
        {
            if (ex.StatusCode == StatusCode.AlreadyExists
             || ex.StatusCode == StatusCode.NotFound
             || ex.StatusCode == StatusCode.FailedPrecondition
             || ex.StatusCode == StatusCode.PermissionDenied)
            {
                _logger.LogInformation(
                    "gRPC бизнес-ошибка ({StatusCode}): {Detail}",
                    ex.StatusCode,
                    ex.Status.Detail);
            }
            else
            {
                _logger.LogError(
                    ex,
                    "gRPC вызов завершился с ошибкой ({StatusCode}): {Detail}",
                    ex.StatusCode,
                    ex.Status.Detail);
            }
            if (context.Response.HasStarted) return;

            await HandleRpcExceptionAsync(context, ex);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Ресурс не найден");
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            var errorResponse = RestApiResponseBuilder<object>.Fail(ex.Message, property: ErrorMessagesConstants.NotFound);
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Необработанное исключение");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var errorResponse = RestApiResponseBuilder<object>.Fail("Внутренняя ошибка сервера", property: ErrorMessagesConstants.InternalServerError);
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }

    private static Task HandleRpcExceptionAsync(HttpContext context, RpcException ex)
    {
        context.Response.StatusCode = ex.StatusCode switch
        {
            StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
            StatusCode.NotFound => StatusCodes.Status404NotFound,
            StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
            StatusCode.AlreadyExists => StatusCodes.Status409Conflict,
            StatusCode.FailedPrecondition => StatusCodes.Status412PreconditionFailed,
            _ => StatusCodes.Status500InternalServerError
        };

        var failure = Failure.Create(ex.Message, ex.Status.StatusCode.ToString());
        var response = RestApiResponse<object>.Fail(failure);

        context.Response.ContentType = MediaTypeNames.Application.Json;

        return context.Response.WriteAsJsonAsync(response);
    }
}
