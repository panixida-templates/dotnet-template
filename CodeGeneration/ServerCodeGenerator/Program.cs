using System;
using System.Collections.Generic;

using CodeGeneration.ServerCodeGenerator.Configs;
using CodeGeneration.ServerCodeGenerator.Core;

using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

var jobs = config.GetSection("Jobs").Get<List<JobConfig>>() ?? throw new InvalidOperationException("Jobs не найдены");

foreach (var job in jobs)
{
    CodeGenerator.Run(job);
}

Console.WriteLine("Генерация завершена");
