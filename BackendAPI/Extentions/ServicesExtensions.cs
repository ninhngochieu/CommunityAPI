using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace BackendAPI.Extentions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddServicesExtensions(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}
