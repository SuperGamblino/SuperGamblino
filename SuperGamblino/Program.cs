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
            Console.WriteLine("Loading configs.");
            Config.LoadConfig();
            DiscordConfiguration cfg = new DiscordConfiguration
            {
                Token = Config.token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                UseInternalLogHandler = true
            };
            Console.WriteLine("Starting client");
            DiscordClient client = new DiscordClient(cfg);

            CommandsNextModule commands = client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = Config.prefix
            });
            Console.WriteLine("Loading Eventhandlers.");
            EventHandler eventHandler = new EventHandler(client);

            client.Ready += eventHandler.OnReady;
            client.ClientErrored += eventHandler.OnClientError;
            Console.WriteLine("Initializing commands.");
            //Initialize commands
            commands.RegisterCommands<RouletteCommand>();
            commands.RegisterCommands<CoinflipCommand>();
            commands.RegisterCommands<SearchCommand>();
            commands.RegisterCommands<CreditsCommand>();
            commands.RegisterCommands<GlobalTopCommand>();
            commands.CommandErrored += eventHandler.OnCommandError;
            Console.WriteLine("Connecting to database...");
            Database.SetConnectionString(Config.dbAddress, Config.dbPort, Config.dbName, Config.dbUsername, Config.dbPass);
            Database.SetupTables();
            Database.SetupProcedures();

            await client.ConnectAsync();
            Console.WriteLine("Bot is ready!");
            await Task.Delay(-1);
        }
    }
}
