namespace SuperGamblino.Core.Configuration
{
    public class Config
    {
        public BotSettings BotSettings { get; set; }

        public ColorSettings ColorSettings { get; set; }

        public DatabaseSettings DatabaseSettings { get; set; }
        public bool IsContainerized { get; set; } = false;

        public bool IsValid()
        {
            if (BotSettings == null || ColorSettings == null || DatabaseSettings == null) return false;

            return BotSettings.IsValid() && ColorSettings.IsValid() && DatabaseSettings.IsValid();
        }
    }

    public class BotSettings
    {
        public string Token { get; set; }
        public string Prefix { get; set; }
        public string TopGgToken { get; set; }

        internal bool IsValid()
        {
            return !(string.IsNullOrWhiteSpace(Token) ||
                     string.IsNullOrWhiteSpace(Prefix));
        }
    }

    public class ColorSettings
    {
        public string Info { get; set; }
        public string Success { get; set; }
        public string Warning { get; set; }

        internal bool IsValid()
        {
            return !(string.IsNullOrWhiteSpace(Info) ||
                     string.IsNullOrWhiteSpace(Success) ||
                     string.IsNullOrWhiteSpace(Warning));
        }
    }

    public class DatabaseSettings
    {
        public string Address { get; set; }
        public int Port { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        internal bool IsValid()
        {
            return !(string.IsNullOrWhiteSpace(Address) ||
                     string.IsNullOrWhiteSpace(Name) ||
                     string.IsNullOrWhiteSpace(Username));
        }

        //TODO Maybe add here GetConnectionString instead of separate class?
    }
}