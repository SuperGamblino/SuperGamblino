using Microsoft.Extensions.Logging;

namespace SuperGamblino.DatabaseConnectors
{
    public abstract class DatabaseConnector
    {
        protected readonly ILogger Logger;
        protected readonly string ConnectionString;
        
        public DatabaseConnector(ILogger logger, ConnectionString connectionString)
        {
            Logger = logger;
            ConnectionString = connectionString.GetConnectionString();
        }
    }
}