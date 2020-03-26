using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace SuperGamblino
{
    internal class EventHandler
    {
        private readonly Config _config;
        private readonly DiscordClient _discordClient;

        public EventHandler(DiscordClient client, Config config)
        {
            _discordClient = client;
            _config = config;
        }

        internal Task OnReady(ReadyEventArgs e)
        {
            _discordClient.UpdateStatusAsync(new DiscordGame("Gambling House"), UserStatus.Online);
            return Task.CompletedTask;
        }

        internal Task OnClientError(ClientErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        internal Task OnCommandError(CommandErrorEventArgs e)
        {
            switch (e.Exception)
            {
                case CommandNotFoundException _:
                    return Task.CompletedTask;
                case ChecksFailedException _:
                {
                    foreach (var attr in ((ChecksFailedException) e.Exception).FailedChecks)
                    {
                        DiscordEmbed error = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor(_config.ColorSettings.Warning),
                            Description = ParseFailedCheck(attr)
                        };
                        e.Context?.Channel?.SendMessageAsync("", false, error);
                    }

                    return Task.CompletedTask;
                }

                default:
                {
                    e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "SuperGamblino",
                        $"Exception occured: {e.Exception.GetType()}: {e.Exception}", DateTime.UtcNow);
                    DiscordEmbed error = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor(_config.ColorSettings.Warning),
                        Description = "Internal error occured, please report this to the developer."
                    };
                    e.Context?.Channel?.SendMessageAsync("", false, error);
                    return Task.CompletedTask;
                }
            }
        }

        private string ParseFailedCheck(CheckBaseAttribute attr)
        {
            switch (attr)
            {
                case CooldownAttribute _:
                    return "You cannot do that so often!";
                case RequireOwnerAttribute _:
                    return "Only the server owner can use that command!";
                case RequirePermissionsAttribute _:
                    return "You don't have permission to do that!";
                case RequireRolesAttributeAttribute _:
                    return "You do not have a required role!";
                case RequireUserPermissionsAttribute _:
                    return "You don't have permission to do that!";
                case RequireNsfwAttribute _:
                    return "This command can only be used in an NSFW channel!";
                default:
                    return "Unknown Discord API error occured, please try again later.";
            }
        }
    }
}