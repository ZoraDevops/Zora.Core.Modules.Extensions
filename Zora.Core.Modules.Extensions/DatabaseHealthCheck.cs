using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using Zora.Modules.Extensions.Helpers;

namespace Zora.Core.Modules.Extensions
{
    public class DatabaseHealthCheck : IHealthCheck 
    {
        private readonly string connectionString;

        public DatabaseHealthCheck(IOptions<ConnectionSettings> dbSettings)
        {
            connectionString = dbSettings.Value.ConnectionString;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {            
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT 1";
                        await command.ExecuteScalarAsync(cancellationToken);
                    }
                }
                return HealthCheckResult.Healthy("Database is responding normally.");
            }
            catch (SqlException ex)
            {
                return HealthCheckResult.Unhealthy($"Database is unhealthy. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"An unexpected error occurred. Error: {ex.Message}");
            }
        }
    }
}
    
