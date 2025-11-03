using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace Api.Extensions.Configurations;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, string serviceName)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = Environment.GetEnvironmentVariable("APP_VERSION") ?? "1.0",
                Title = "Api",
                Description = $"{serviceName}. Версия: {Environment.GetEnvironmentVariable("APP_VERSION") ?? "1.0"}",
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
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

            options.AddEnumsWithValuesFixFilters(item =>
            {
                item.ApplySchemaFilter = true;
                item.IncludeDescriptions = true;
            });
        });

        return services;
    }

    public static void UseSwaggerAndSwaggerUI(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}
