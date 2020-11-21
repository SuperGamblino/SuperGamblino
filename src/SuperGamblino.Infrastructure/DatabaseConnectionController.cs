using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SuperGamblino.Core.Configuration;

namespace SuperGamblino.Infrastructure
{
    public class DatabaseConnectionController
    {
        public static async Task ControlDatabaseConnection(IServiceProvider serviceProvider,
            CancellationTokenSource token)
        {
            var connectionString = serviceProvider.GetRequiredService<ConnectionString>();
            var logger = serviceProvider.GetRequiredService<ILogger<DatabaseConnectionController>>();
            logger.LogInformation($"[{nameof(DatabaseConnectionController)}] - Ready for work.");
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    logger.LogInformation(
                        $"[{nameof(DatabaseConnectionController)}] - Cancellation request received. Shutting the app down.");
                    break;
                }

                if (!await DatabaseHelpers.CheckConnection(connectionString))
                {
                    logger.LogCritical(
                        $"[{nameof(DatabaseConnectionController)}] - Database connection failed! Shutting the app down.");
                    token.Cancel();
                    break;
                }

                await Task.Delay(10_000);
            }
        }
    }
}