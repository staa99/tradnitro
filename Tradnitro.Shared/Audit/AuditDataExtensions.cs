using Microsoft.Extensions.DependencyInjection;


namespace Tradnitro.Shared.Audit
{
    public static class AuditDataExtensions
    {
        public static IServiceCollection RegisterAudits(this IServiceCollection services)
        {
            services.AddScoped<IAuditSecurityData, AuditData>();
            return services;
        }
    }
}