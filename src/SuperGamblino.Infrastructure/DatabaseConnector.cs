using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SuperGamblino.Core;
using SuperGamblino.Core.Configuration;

namespace SuperGamblino.Infrastructure
{
    public abstract class DatabaseConnector
    {
        protected readonly string ConnectionString;
        protected readonly ILogger Logger;
        protected readonly IMemoryCache MemoryCache;

        public DatabaseConnector(ILogger logger, ConnectionString connectionString, IMemoryCache memoryCache)
        {
            Logger = logger;
            MemoryCache = memoryCache;
            ConnectionString = connectionString.GetConnectionString();
        }
    }
}