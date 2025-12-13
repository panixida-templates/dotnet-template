using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Api.Extensions.Configurations;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, string serviceName)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            var appVersion = Environment.GetEnvironmentVariable("APP_VERSION") ?? "1.0";

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = appVersion,
                Title = serviceName,
                Description = $"{serviceName}. Версия: {appVersion}",
            });

            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("bearer", document),
                    new List<string>()
                }
            });

            var basePath = AppContext.BaseDirectory;
            var projectName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(basePath, projectName);
            if (!File.Exists(xmlPath))
            {
                using var fileStream = File.Create(xmlPath);
                using var streamWriter = new StreamWriter(fileStream);
                streamWriter.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?><doc></doc>");
            }

            options.IncludeXmlComments(xmlPath);

            options.EnableAnnotations();
        });

        return services;
    }

    public static void UseSwaggerAndSwaggerUI(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}
