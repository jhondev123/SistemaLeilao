using Scalar.AspNetCore;
using Serilog;
using SistemaLeilao.API.Endpoints;
using SistemaLeilao.API.Middlewares;
using SistemaLeilao.Core.Application;
using SistemaLeilao.Infrastructure;
using SistemaLeilao.Infrastructure.Extensions;
using SistemaLeilao.Infrastructure.Hubs;
using SistemaLeilao.Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

LoggingConfiguration.ConfigureSerilog(builder.Host, builder.Configuration);
try
{
    Log.Information("Iniciando o Sistema de Leilão...");

    // Camadas do Sistema
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddMessaging(builder.Configuration);

    // Exception Handling
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddOpenApi();
    builder.Services.AddSignalR();

    var app = builder.Build();

    // Middleware de log de requisições HTTP
    app.UseSerilogRequestLogging();

    app.UseAuthentication();
    app.UseAuthorization();

    // HealthCheck ou Global Exception Handler (se necessário)
    app.UseExceptionHandler();

    // Endpoints
    app.MapAuctionEndpoints();
    app.MapBidEndpoints();
    app.MapAuthEndpoints();
    app.MapAdminEndpoints();

    app.MapHub<AuctionHub>("/hubs/auction");

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();

        await app.ApplyMigrations();
    }

    app.UseHttpsRedirection();

    Log.Information("Sistema de Leilão pronto. Iniciando host...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação falhou ao iniciar.");
}
finally
{
    Log.CloseAndFlush();
}