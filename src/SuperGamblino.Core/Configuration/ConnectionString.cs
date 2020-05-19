﻿namespace SuperGamblino.Core.Configuration
{
    public class ConnectionString
    {
        private readonly string _connectionString;

        public ConnectionString(Config config)
        {
            _connectionString = $"Server={config.DatabaseSettings.Address};Database={config.DatabaseSettings.Name};" +
                                $"Port={config.DatabaseSettings.Port};Uid={config.DatabaseSettings.Username};" +
                                $"Pwd={config.DatabaseSettings.Password}";
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}