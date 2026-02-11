using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure.Logging
{
    public static class LoggingConfiguration
    {
        public static void ConfigureSerilog(IHostBuilder host, IConfiguration configuration)
        {
            host.UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.OpenTelemetry(options =>
                    {
                        options.Endpoint = configuration["Otlp:Endpoint"] ?? "http://localhost:4317";
                        options.ResourceAttributes = new Dictionary<string, object>
                        {
                            ["service.name"] = "SistemaLeilao.API"
                        };
                    });
            });
        }
    }
}
