using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.Helpers;
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

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            if (!configuration.GetSection("Settings").Exists())
            {
                logger.LogError("Couldn't find any settings! Checkout our github to find out how to configure this bot!");
                Thread.Sleep(30); //Without it logger don't have enough time to display the error to console
                Environment.Exit(1);
            }
            
            var config = new Config();

            configuration.Bind("Settings", config);

            var connectionString = new ConnectionString(config);

            var dependencies = new DependencyCollectionBuilder()
                .AddInstance(logger)
                .AddInstance(config)
                .AddInstance(connectionString)
                .AddInstance(new HttpClient())
                .AddDatabaseConnectors()
                .Add<MessagesHelper>()
                .Add<BetSizeParser>()
                .AddCommandLogics()
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
            var connectionString = dependencyCollection.GetDependency<ConnectionString>();
            var coinDrop = dependencyCollection.GetDependency<CoindropConnector>();
            var msg = dependencyCollection.GetDependency<MessagesHelper>();
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
            var eventHandler = new EventHandler(client, conf, coinDrop, msg);

            client.Ready += eventHandler.OnReady;
            client.ClientErrored += eventHandler.OnClientError;
            client.MessageCreated += eventHandler.MessageCreated;
            logger.LogInformation("Event handlers were registered successfully.");


            logger.LogInformation("Registering commands...");
            commands.AddCommands();
            commands.CommandErrored += eventHandler.OnCommandError;
            logger.LogInformation("All commands registered successfully.");

            var connectedSuccessfully = false;
            logger.LogInformation("Starting the DB connection...");
            while (!connectedSuccessfully)
                try
                {
                    await DatabaseHelpers.SetupTables(connectionString.GetConnectionString());
                    await DatabaseHelpers.SetupProcedures(connectionString.GetConnectionString());
                    connectedSuccessfully = true;
                    logger.LogInformation("DB loaded successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Couldn't connect to the DB! Waiting 15 seconds to retry...");
                    Thread.Sleep(15000);
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