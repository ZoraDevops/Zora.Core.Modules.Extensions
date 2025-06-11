using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Zora.Core.Modules.Extensions
{
    public static class HealthCheck 
    { 
        public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks();            
            return builder;
        }
        public static IHostApplicationBuilder AddDatabaseHealthChecks(this IHostApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks()
                            .AddCheck<DatabaseHealthCheck>("DatabaseHealthCheck");
            return builder;
        }  
    }
}
