using BlazorEssentials.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEssentials.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBlazorEssentials(this IServiceCollection services)
        {
            services.AddScoped<IStateManager, StateManager>();
            services.AddScoped<IStorageManager, StorageManager>();
            services.AddScoped<ILocationService, LocationService>();

            return services;
        }
    }
}
