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
using LogLevel = DSharpPlus.LogLevel;

namespace SuperGamblino.Discord
{
    public class DiscordBot
    {
        public static async Task DiscordMainAsync(ServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<DiscordBot>>();
            var configuration = serviceProvider.GetRequiredService<Config>();
            logger.LogInformation($"[{nameof(DiscordBot)}] - Client is starting...");
            
            var discordConfiguration = new DiscordConfiguration
            {
                Token = configuration.BotSettings.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Info
            };
            
            var commandsConfiguration = new CommandsNextConfiguration()
            {
                CaseSensitive = false,
                Services = serviceProvider,
                StringPrefixes = new []
                {
                    configuration.BotSettings.Prefix
                }
            };
            
            var client = new DiscordClient(discordConfiguration);

            logger.LogInformation($"[{nameof(DiscordBot)}] - Registering event handlers...");
            
            var eventHandlers = new EventHandler(client, configuration, 
                serviceProvider.GetRequiredService<CoindropConnector>(),
                serviceProvider.GetRequiredService<Messages.MessagesHelper>());

            client.MessageCreated += eventHandlers.MessageCreated;
            client.ClientErrored += eventHandlers.OnClientError;
            
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
            {
                throw new Exception("Verification of DB connection was unsuccessful!");
            }
            
            logger.LogInformation($"[{nameof(DiscordBot)}] - DB connection verification successful.");
            
            try
            {
                await client.ConnectAsync(new DiscordActivity("Gambling House", ActivityType.Playing), UserStatus.Online);
                logger.LogInformation($"[{nameof(DiscordBot)}] - Successfully started the bot.");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Exception occured during logging in into Discord servers! Check you bot token!");
                throw;
            }
            
            try
            {
                await Task.Delay(-1, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation($"{nameof(DiscordBot)} is shutting down...");
            }
        }
    }
}