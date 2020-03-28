using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SuperGamblino.Commands;
using SuperGamblino.Helpers;
using SuperGamblino.Properties;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace SuperGamblino
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var logger = LoggerFactory.Create(builder => builder
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                .AddConsole()).CreateLogger<Program>();


            if (!File.Exists("./config.json"))
            {
                logger.LogError(
                    "There was no config.json file found so we created new default one. Please fill it up with info and start this bot again!");
                File.WriteAllText("./config.json", Encoding.UTF8.GetString(Resources.defaultconfig));
                Environment.Exit(1);
            }

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json", false, true)
                .Build();

            var config = new Config();

            configuration.Bind("Settings", config);

            var dependencies = new DependencyCollectionBuilder()
                .AddInstance(logger)
                .AddInstance(config)
                .Add<Database>()
                .Add<Messages>()
                .Add<BetSizeParser>()
                .Build();
            try
            {
                new Program().MainAsync(dependencies).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Some error occured! Check your configuration file!");
                Thread.Sleep(30); //Without it logger don't have enough time to display the error to console
                Environment.Exit(1);
            }
        }

        private async Task MainAsync(DependencyCollection dependencyCollection)
        {
            var logger = dependencyCollection.GetDependency<ILogger>();
            var conf = dependencyCollection.GetDependency<Config>();
            var db = dependencyCollection.GetDependency<Database>();

            var cfg = new DiscordConfiguration
            {
                Token = conf.BotSettings.Token,
                TokenType = TokenType.Bot,

                AutoReconnect = true,
                UseInternalLogHandler = true
            };

            logger.LogInformation("Starting discord bot...");
            var client = new DiscordClient(cfg);

            var commands = client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = conf.BotSettings.Prefix,
                Dependencies = dependencyCollection
            });

            logger.LogInformation("Registering event handlers...");
            var eventHandler = new EventHandler(client, conf);

            client.Ready += eventHandler.OnReady;
            client.ClientErrored += eventHandler.OnClientError;
            logger.LogInformation("Event handlers were registered successfully.");


            logger.LogInformation("Registering commands...");
            //Initialize commands
            commands.RegisterCommands<RouletteCommand>();
            commands.RegisterCommands<CoinflipCommand>();
            // commands.RegisterCommands<SearchCommand>(); Removed for balance
            commands.RegisterCommands<CreditsCommand>();
            commands.RegisterCommands<GlobalTopCommand>();
            commands.RegisterCommands<HourlyReward>();
            commands.RegisterCommands<DailyReward>();
            commands.RegisterCommands<Cooldown>();
            commands.RegisterCommands<WorkReward>();
            commands.RegisterCommands<GamesHistory>();
            commands.RegisterCommands<ShowProfile>();
            commands.CommandErrored += eventHandler.OnCommandError;
            logger.LogInformation("All commands registered successfully.");

            logger.LogInformation("Starting the DB connection...");
            db.SetConnectionString(conf.DatabaseSettings.Address, conf.DatabaseSettings.Port,
                conf.DatabaseSettings.Name, conf.DatabaseSettings.Username, conf.DatabaseSettings.Password);
            try
            {
                await db.SetupTables();
                await db.SetupProcedures();
                logger.LogInformation("DB loaded successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception occured during DB connection creation!");
                Environment.Exit(1);
            }

            try
            {
                await client.ConnectAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception occured during logging in into Discord servers! Check you bot token!");
                Environment.Exit(1);
            }

            logger.LogInformation("Discord bot loaded.");
            await Task.Delay(-1);
        }
    }
}