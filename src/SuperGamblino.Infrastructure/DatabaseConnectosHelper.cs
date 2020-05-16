using Microsoft.Extensions.DependencyInjection;
using SuperGamblino.Infrastructure.Connectors;

namespace SuperGamblino.Infrastructure
{
    public static class DatabaseConnectorsHelper
    {
        /// <summary>
        ///     Should be called AFTER ConnectionString service and Logger service
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddDatabaseConnectors(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddTransient<BlackjackConnector>()
                .AddTransient<GameHistoryConnector>()
                .AddTransient<CoindropConnector>()
                .AddTransient<UsersConnector>();
        }
    }
}