using Microsoft.Extensions.Logging;

namespace SuperGamblino.DatabaseConnectors
{
    public abstract class DatabaseConnector
    {
        protected readonly string ConnectionString;
        protected readonly ILogger Logger;

        public DatabaseConnector(ILogger logger, ConnectionString connectionString)
        {
            Logger = logger;
            ConnectionString = connectionString.GetConnectionString();
        }
    }
}