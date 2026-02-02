using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SistemaLeilao.Core.Domain.Enums;
using SistemaLeilao.Infrastructure.Extensions;
using SistemaLeilao.Infrastructure.Indentity;
using SistemaLeilao.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure.Persistence.Seeders
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            var logger = serviceProvider.GetRequiredService<ILogger<PostgresDbContext>>();


            string[] roleNames = {
                RoleEnum.Admin.GetDescription(),
                RoleEnum.Auctioneer.GetDescription(),
                RoleEnum.Bidder.GetDescription()
            };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    logger.LogInformation($"Inserindo role {roleName}");
                    await roleManager.CreateAsync(new Role { Name = roleName });
                }
            }
            await CreateAdminUser(serviceProvider, configuration, userManager,logger);

        }
        private static async Task CreateAdminUser(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            UserManager<User> userManager,
            ILogger logger)
        {
            var adminEmail = configuration["InitialSeeding:AdminEmail"];
            var adminPassword = configuration["InitialSeeding:AdminPassword"];

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            {
                logger.LogWarning("Seeding do Admin pulado: Variáveis de ambiente ADMIN_USER_EMAIL ou ADMIN_USER_PASSWORD não configuradas.");
                return;
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new User { UserName = adminEmail, Email = adminEmail };
                logger.LogInformation($"Criando usuário administrativo");
                var result = await userManager.CreateAsync(newAdmin, adminPassword);

                if (result.Succeeded)
                {
                    logger.LogInformation($"Adicionando cargo ao usuário administrativo");
                    await userManager.AddToRoleAsync(newAdmin, RoleEnum.Admin.GetDescription());
                }
                else
                {
                    logger.LogError("Falha ao criar usuário Admin: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
