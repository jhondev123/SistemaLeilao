using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SistemaLeilao.Core.Application.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SistemaLeilao.Core.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            return services;
        }
    }
}
