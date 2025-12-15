using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Pl.Api.Http.Controllers.Core;
using Pl.Api.Http.Middlewares;

using System.Text.Json.Serialization;

namespace Pl.Api.Http.Composition;

public static class HttpComposition
{
    public static void AddHttp(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.AddControllers()
            .AddApplicationPart(typeof(BaseApiController).Assembly)
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddAuthentication();
        services.AddAuthorization();
    }

    public static void UseHttp(this WebApplication app)
    {
        app.RegisterMiddlewares();

        app.UseRouting();

        app.UseCors("AllowAll");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
    }

    private static void RegisterMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<ExceptionsMiddleware>();
        //app.UseMiddleware<RequestMiddleware>();
        app.UseMiddleware<LoggingMiddleware>();
    }
}
