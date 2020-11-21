using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SuperGamblino.Commands;
using SuperGamblino.Core.Configuration;
using SuperGamblino.Core.Helpers;
using SuperGamblino.Discord;
using SuperGamblino.Infrastructure;
using SuperGamblino.Messages;

namespace SuperGamblino
{
    internal class Program
    {
        private static IServiceProvider _serviceProvider;

        private static Task ConfigureServices()
        {
            var configuration = new Config();
            var conf = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "config.json"), true, false)
                .AddEnvironmentVariables()
                .Build();
            conf.Bind(configuration);
            configuration.IsContainerized = conf.GetValue<bool>("DOTNET_RUNNING_IN_CONTAINER");
            var connectionString = new ConnectionString(configuration);


            _serviceProvider = new ServiceCollection()
                .AddLogging(options =>
                {
                    options
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                        .AddConsole();
                })
                .AddMemoryCache()
                .AddSingleton(configuration)
                .AddSingleton(connectionString)
                .AddTransient<MessagesHelper>()
                .AddTransient<BetSizeParser>()
                .AddTransient<HttpClient>()
                .AddDatabaseConnectors()
                .AddCommands()
                .BuildServiceProvider();
            return Task.CompletedTask;
        }

        private static async Task Main(string[] args)
        {
            await ConfigureServices();
            var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();

            if (!_serviceProvider.GetRequiredService<Config>().IsValid())
                await Exit(ExitCodes.CONFIGURATION_IS_INVALID, "Configuration is not valid!");

            var cancellationToken = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancellationToken.Cancel();
            };

            logger.LogInformation("Setting up the DB...");

            if (_serviceProvider.GetRequiredService<Config>().IsContainerized)
            {
                logger.LogInformation("App is containerized. - Giving some ahead time for DB to kick in...");
                await Task.Delay(10_000);
            }

            var connectionString = _serviceProvider.GetRequiredService<ConnectionString>();

            try
            {
                await DatabaseHelpers.SetupTables(connectionString);
                await DatabaseHelpers.SetupProcedures(connectionString);
                logger.LogInformation("DB is set up.");
            }
            catch (Exception)
            {
                await Exit(ExitCodes.COULD_NOT_CONNECT_TO_DB,
                    "Couldn't connect to DB! Check your connection..");
            }

            try
            {
                await Task.WhenAll(Task.Run(() => DiscordBot.DiscordMainAsync(_serviceProvider, cancellationToken),
                    cancellationToken.Token), Task.Run(
                    () => DatabaseConnectionController.ControlDatabaseConnection(_serviceProvider, cancellationToken),
                    cancellationToken.Token));
            }
            catch (Exception ex)
            {
                cancellationToken.Cancel();
                logger.LogCritical(ex, "Exception occured in one of the clients!");
            }
        }

        private static Task Exit(ExitCodes exitCode, string message, object[] objects = null)
        {
            _serviceProvider.GetRequiredService<ILogger<Program>>().LogCritical(message, objects);
            Task.Delay(30);
            Environment.Exit((int) exitCode);
            return Task.CompletedTask;
        }
    }
}