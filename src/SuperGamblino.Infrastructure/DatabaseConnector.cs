using Microsoft.Extensions.Logging;
using SuperGamblino.Core;
using SuperGamblino.Core.Configuration;

namespace SuperGamblino.Infrastructure
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