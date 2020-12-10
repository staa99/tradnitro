using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Tradnitro.Shared.Cors
{
    public static class CorsExtensions
    {
        private const string TradnitroCorsPolicy = "Tradnitro_CORS_POLICY";


        public static IServiceCollection RegisterCors(this IServiceCollection services, IConfiguration configuration)
        {
            var rootDomain = configuration["AuthSSO:Domain"] ?? throw new ApplicationException("Shared auth is not correctly configured.");
            if (rootDomain.StartsWith('.'))
            {
                rootDomain = rootDomain.Substring(1);
            }

            var origins = configuration["CORS"]
                        ?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                         .Select(s => s.Trim().Replace("{domain}", rootDomain))
                         .Where(s => !string.IsNullOrWhiteSpace(s))
                         .ToArray();

            return services.AddCors(options =>
            {
                options.AddPolicy(TradnitroCorsPolicy,
                                  builder =>
                                  {
                                      builder.WithOrigins(origins)
                                             .AllowAnyHeader()
                                             .WithMethods("GET", "POST")
                                             .SetPreflightMaxAge(TimeSpan.FromSeconds(120))
                                             .AllowCredentials();
                                  });
            });
        }
    }
}