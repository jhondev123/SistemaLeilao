using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Application.Features.Bid.Consumers;
using SistemaLeilao.Core.Application.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Infrastructure.BackgroundServices;
using SistemaLeilao.Infrastructure.Indentity;
using SistemaLeilao.Infrastructure.Persistence.Contexts;
using SistemaLeilao.Infrastructure.Persistence.Repositories;
using SistemaLeilao.Infrastructure.Services.Auth;
using SistemaLeilao.Infrastructure.Services.JwtToken;
using SistemaLeilao.Infrastructure.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<PostgresDbContext>(options =>
                options.UseNpgsql(connectionString, b =>
                    b.MigrationsAssembly(typeof(PostgresDbContext).Assembly.FullName)));

            ConfigureIdentity(services);

            ConfigureJWT(services, configuration);

            ConfigureDependencies(services);

            ConfigureAuthorization(services);

            ConfigureBackgroundServices(services);

            ConfigureHubs(services);

            return services;
        }
        private static void ConfigureIdentity(IServiceCollection services)
        {
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<PostgresDbContext>()
            .AddDefaultTokenProviders();
        }
        private static void ConfigureDependencies(IServiceCollection services)
        {
            ConfigureDependenciesServices(services);

            ConfigureDependenciesRepositories(services);
        }
        private static void ConfigureDependenciesServices(IServiceCollection services)
        {
            services.AddScoped<IJwtTokenGeneratorService, JwtTokenGenerator>();
            services.AddScoped<IAuthService, AuthService>();
        }
        private static void ConfigureDependenciesRepositories(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuctionRepository, AuctionRepository>();
            services.AddScoped<IBidderRepository, BidderRepository>();
            services.AddScoped<IBidRepository, BidRepository>();
            services.AddScoped<IAuctioneerRepository, AuctioneerRepository>();
        }
        private static void ConfigureJWT(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secret = jwtSettings.GetValue<string>("Secret");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
                    ValidAudience = jwtSettings.GetValue<string>("Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!)),
                    ClockSkew = TimeSpan.Zero
                };

                // Especial para SignalR: Permite ler o token da QueryString para WebSockets
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization();
        }
        private static void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Política para administradores totais
                options.AddPolicy(AuthorizationPolicies.AdminOnly, policy =>
                    policy.RequireRole("Admin"));

                // Política para quem pode gerenciar leilões (Admin ou Leiloeiro)
                options.AddPolicy(AuthorizationPolicies.AuctioneerOnly, policy =>
                    policy.RequireRole("Admin", "Auctioneer"));

                // Política para clientes/licitantes
                options.AddPolicy(AuthorizationPolicies.BidderOnly, policy =>
                    policy.RequireRole("Bidder", "Admin"));
            });
        }
        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                // 1. Adiciona automaticamente todos os consumidores do assembly onde está o BidPlacedConsumer
                x.AddConsumers(typeof(BidPlacedConsumer).Assembly);

                x.UsingRabbitMq((context, cfg) =>
                {
                    // 2. Configura o Host (pegando do appsettings.json)
                    cfg.Host(configuration.GetConnectionString("RabbitMQ"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    // 3. Configura as filas automaticamente seguindo o padrão do MassTransit
                    cfg.ConfigureEndpoints(context);

                    // 4. Política de Retry Global para erros de concorrência (Lock Otimista)
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromMilliseconds(200)));
                });
            });

            return services;
        }
        private static void ConfigureBackgroundServices(IServiceCollection services)
        {
            services.AddHostedService<AuctionStatusWorker>();
        }
        private static void ConfigureHubs(IServiceCollection services)
        {
            services.AddScoped<IAuctionNotificationService, AuctionNotificationService>();
        }
    }
}
