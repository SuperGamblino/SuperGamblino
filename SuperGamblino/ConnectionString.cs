namespace SuperGamblino
{
    public class ConnectionString
    {
        private readonly string _connectionString;

        public ConnectionString(Config config)
        {
            _connectionString = $"server={config.DatabaseSettings.Address};database={config.DatabaseSettings.Name};" +
                                $"port={config.DatabaseSettings.Port};userid={config.DatabaseSettings.Username};" +
                                $"password={config.DatabaseSettings.Password}";
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}