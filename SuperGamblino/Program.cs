using DSharpPlus;
using DSharpPlus.CommandsNext;
using SuperGamblino.Commands;
using System;
using System.Threading.Tasks;

namespace SuperGamblino
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }
        private async Task MainAsync()
        {
            Config.LoadConfig();
            DiscordConfiguration cfg = new DiscordConfiguration
            {
                Token = Config.token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                UseInternalLogHandler = true
            };
            DiscordClient client = new DiscordClient(cfg);

            CommandsNextModule commands = client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = Config.prefix
            });

            EventHandler eventHandler = new EventHandler(client);

            client.Ready += eventHandler.OnReady;

            //Initialize commands
            commands.RegisterCommands<HelpCommand>();
            commands.RegisterCommands<CoinflipCommand>();

            Console.WriteLine("Connecting to database...");
            Database.SetConnectionString(Config.dbAddress, Config.dbPort, Config.dbName, Config.dbUsername, Config.dbPass);
            Database.SetupTables();

            await client.ConnectAsync();
            Console.WriteLine("Bot is ready!");
            await Task.Delay(-1);
        }
    }
}
