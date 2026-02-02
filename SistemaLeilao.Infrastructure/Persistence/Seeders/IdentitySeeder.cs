using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SistemaLeilao.Core.Domain.Enums;
using SistemaLeilao.Infrastructure.Extensions;
using SistemaLeilao.Infrastructure.Indentity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure.Persistence.Seeders
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            string[] roleNames = {
                RoleEnum.Admin.GetDescription(),
                RoleEnum.Auctioneer.GetDescription(),
                RoleEnum.Bidder.GetDescription()
            };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
            }
            await CreateAdminUser(serviceProvider, configuration, userManager);

        }
        private static async Task CreateAdminUser(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            UserManager<User> userManager)
        {
            var adminEmail = configuration["InitialSeeding:AdminEmail"];
            var adminPassword = configuration["InitialSeeding:AdminPassword"];
            var logger = serviceProvider.GetRequiredService<ILogger>();

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            {
                logger.LogWarning("Seeding do Admin pulado: Variáveis de ambiente ADMIN_USER_EMAIL ou ADMIN_USER_PASSWORD não configuradas.");
                return;
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new User { UserName = adminEmail, Email = adminEmail };

                var result = await userManager.CreateAsync(newAdmin, adminPassword);

                if (result.Succeeded)
                {
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
