using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SuperGamblino.Core.Configuration;

namespace SuperGamblino.Infrastructure
{
    public class DatabaseConnectionController
    {
        public static async Task ControlDatabaseConnection(ServiceProvider serviceProvider, CancellationTokenSource token)
        {
            var connectionString = serviceProvider.GetRequiredService<ConnectionString>();
            var logger = serviceProvider.GetRequiredService<ILogger<DatabaseConnectionController>>();
            while (true)
            {
                if (!await DatabaseHelpers.CheckConnection(connectionString))
                {
                    logger.LogCritical("Database connection failed!!!");
                    token.Cancel();
                    break;
                }
                else
                {
                    await Task.Delay(10_000);
                }
            }
        }
    }
}