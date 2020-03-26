namespace SuperGamblino
{
    public class Config
    {
        public BotSettings BotSettings { get; set; }

        public ColorSettings ColorSettings { get; set; }

        public DatabaseSettings DatabaseSettings { get; set; }
    }

    public class BotSettings
    {
        public string Token { get; set; }
        public string Prefix { get; set; }
    }

    public class ColorSettings
    {
        public string Info { get; set; }
        public string Success { get; set; }
        public string Warning { get; set; }
    }

    public class DatabaseSettings
    {
        public string Address { get; set; }
        public int Port { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}