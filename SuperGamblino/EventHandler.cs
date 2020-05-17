using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino
{
    internal class EventHandler
    {
        private readonly CoindropConnector _coindropConnector;
        private readonly Config _config;
        private readonly DiscordClient _discordClient;
        private readonly MessagesHelper _messagesHelper;

        public EventHandler(DiscordClient client, Config config, CoindropConnector coindropConnector,
            MessagesHelper messagesHelper)
        {
            _discordClient = client;
            _config = config;
            _coindropConnector = coindropConnector;
            _messagesHelper = messagesHelper;
        }

        internal Task OnReady(ReadyEventArgs e)
        {
            _discordClient.UpdateStatusAsync(new DiscordGame("Gambling House"), UserStatus.Online);
            return Task.CompletedTask;
        }

        internal async Task MessageCreated(MessageCreateEventArgs e)
        {
            if (!e.Author.IsBot)
            {
                var sId = e.Message.Id.ToString();
                var id = Convert.ToInt32(sId.Substring(sId.Length - 4, 4)) + 1; //1-10000
                if (id <= 100) //1% chance
                { 
                    var reward = id >= 65 ? 25 : 100;
                    var claimId = await _coindropConnector.AddCoindrop(e.Channel.Id, reward);
                    await e.Message.RespondAsync("", false, _messagesHelper.CoinDropAlert(claimId));
                }
            }
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
            return attr switch
            {
                CooldownAttribute _ => "You cannot do that so often!",
                RequireOwnerAttribute _ => "Only the server owner can use that command!",
                RequirePermissionsAttribute _ => "You don't have permission to do that!",
                RequireRolesAttributeAttribute _ => "You do not have a required role!",
                RequireUserPermissionsAttribute _ => "You don't have permission to do that!",
                RequireNsfwAttribute _ => "This command can only be used in an NSFW channel!",
                _ => "Unknown Discord API error occured, please try again later."
            };
        }
    }
}