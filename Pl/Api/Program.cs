using Api.Extensions.Configurations;
using Api.Middlewares;

using Bl.DependencyInjection;

using Common.Constants;

using Dal.DependencyInjection;

using Logging.OpenSearch;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

#region Custom Extensions

builder.Services.ConfigureGrpcClients(builder.Configuration);

builder.Services.UseBl();
builder.Services.UseDal(builder.Configuration);

builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);
builder.Services.AddSwagger(AppsettingsKeysConstants.ServiceName);

if (!builder.Environment.IsEnvironment(EnvironmentConstants.Test))
{
    //builder.Services.AddPrometheusMetrics();
    builder.Host.UseSerilog(OpenSearchConfiguration.ConfigureOpenSearch(builder.Configuration));
}

#endregion

builder.Services.AddControllers()
    .AddJsonOptions(jsonOptions =>
    {
        jsonOptions.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

#region Middlewares

app.UseMiddleware<ExceptionsMiddleware>();
//app.UseMiddleware<RequestMiddleware>();
app.UseMiddleware<LoggingMiddleWare>();

#endregion

app.UseHttpsRedirection();
app.UseRouting();

#region Custom Extensions

if (!builder.Environment.IsEnvironment(EnvironmentConstants.Test))
{
    //app.UsePrometheusMetrics();
}

app.UseSwaggerAndSwaggerUI();
app.UseAuthenticationAndAuthorization();

#endregion

app.MapControllers();

await app.RunAsync();