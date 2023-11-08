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
        public static IServiceCollection AddBlazorEssentials(this IServiceCollection services, string BaseURL)
        {
            //services.AddScoped<IStorageManager, StorageManager>();
            //services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<Interop>();
            services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(BaseURL) });
            //services.AddScoped<IAuthService, AuthService>();

            return services;
        }

        
    }
}
