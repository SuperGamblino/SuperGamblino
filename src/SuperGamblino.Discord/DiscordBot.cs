using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SuperGamblino.Core.Configuration;
using SuperGamblino.Infrastructure;
using SuperGamblino.Infrastructure.Connectors;

namespace SuperGamblino.Discord
{
    public class DiscordBot
    {
        public static async Task DiscordMainAsync(IServiceProvider serviceProvider,
            CancellationTokenSource cancellationToken)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<DiscordBot>>();
            var configuration = serviceProvider.GetRequiredService<Config>();
            logger.LogInformation($"[{nameof(DiscordBot)}] - Client is starting...");

            var discordConfiguration = new DiscordConfiguration
            {
                Token = configuration.BotSettings.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Information
            };

            var commandsConfiguration = new CommandsNextConfiguration
            {
                CaseSensitive = false,
                Services = serviceProvider,
                StringPrefixes = new[]
                {
                    configuration.BotSettings.Prefix
                }
            };
            try
            {
                var client = new DiscordClient(discordConfiguration);

                logger.LogInformation($"[{nameof(DiscordBot)}] - Registering event handlers...");

                var eventHandlers = new EventHandler(client, configuration,
                    serviceProvider.GetRequiredService<CoindropConnector>(),
                    serviceProvider.GetRequiredService<Messages.MessagesHelper>());

                client.MessageCreated += eventHandlers.MessageCreated;

                logger.LogInformation($"[{nameof(DiscordBot)}] - Event handlers registered.");


                logger.LogInformation(
                    $"[{nameof(DiscordBot)}] - Registering commands...");

                var commands = client.UseCommandsNext(commandsConfiguration);
                commands.AddCommands();

                commands.CommandErrored += eventHandlers.OnCommandError;

                logger.LogInformation(
                    $"[{nameof(DiscordBot)}] - Finished registering commands...");


                logger.LogInformation($"[{nameof(DiscordBot)}] - Verifying DB connection...");

                if (!await DatabaseHelpers.CheckConnection(serviceProvider.GetRequiredService<ConnectionString>()))
                    throw new Exception("Verification of DB connection was unsuccessful!");

                logger.LogInformation($"[{nameof(DiscordBot)}] - DB connection verification successful.");

                try
                {
                    await client.ConnectAsync(new DiscordActivity("Gambling House", ActivityType.Playing),
                        UserStatus.Online);
                    logger.LogInformation($"[{nameof(DiscordBot)}] - Successfully started the bot.");
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex,
                        "Exception occured during logging in into Discord servers! Check you bot token!");
                    throw;
                }

                try
                {
                    await Task.Delay(-1, cancellationToken.Token);
                }
                catch (OperationCanceledException)
                {
                    logger.LogInformation($"{nameof(DiscordBot)} is shutting down...");
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, $"[{nameof(DiscordBot)}] - Uncaught exception occured!");
                cancellationToken.Cancel();
            }
        }
    }
}