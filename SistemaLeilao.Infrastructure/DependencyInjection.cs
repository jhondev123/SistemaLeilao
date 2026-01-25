using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SistemaLeilao.Core.Application.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Infrastructure.Indentity;
using SistemaLeilao.Infrastructure.Persistence.Contexts;
using SistemaLeilao.Infrastructure.Persistence.Repositories;
using SistemaLeilao.Infrastructure.Services.Auth;
using SistemaLeilao.Infrastructure.Services.JwtToken;
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

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuctionRepository, AuctionRepository>();
            services.AddScoped<IBidderRepository, BidderRepository>();
            services.AddScoped<IBidRepository, BidRepository>();

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
    }
}
