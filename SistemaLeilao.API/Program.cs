using Scalar.AspNetCore;
using Serilog;
using SistemaLeilao.API.Endpoints;
using SistemaLeilao.API.Middlewares;
using SistemaLeilao.Core.Application;
using SistemaLeilao.Infrastructure;
using SistemaLeilao.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Iniciando o Sistema de Leilão...");


    builder.Services.AddApplication();

    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddOpenApi();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.MapAuctionEndpoints();
    app.MapAuthEndpoints();
    app.MapAdminEndpoints();

    if (app.Environment.IsDevelopment())
    {
        Log.Information("Executando configurações para o ambiente de desenvolvimento...");

        Log.Information("Configurando recursos da API...");

        app.MapOpenApi();
        app.MapScalarApiReference();

        Log.Information("Configurando banco de dados...");
        await app.ApplyMigrations();
    }

    Log.Information("Configurando Https");
    app.UseHttpsRedirection();
    Log.Information("Configurando Authentication");
    app.UseAuthentication();
    Log.Information("Configurando Authorization");
    app.UseAuthorization();

    await app.StartAsync();

    var serverAddresses = string.Join(", ", app.Urls);

    if (string.IsNullOrEmpty(serverAddresses))
    {
        Log.Information("Aplicação em funcionamento (Endereços gerenciados pelo Host/Kestrel)");
    }
    else
    {
        Log.Information("Sistema de Leilão pronto e ouvindo em: {Addresses}", serverAddresses);
        Log.Information("Link do Scalar em: {Addresses}", serverAddresses + "/scalar/v1");
    }

    Log.Information("Aplicação em funcionamento...");
    await app.WaitForShutdownAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação falhou ao iniciar.");
}
finally
{
    Log.CloseAndFlush();
}

