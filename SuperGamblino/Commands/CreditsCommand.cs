using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.Commands
{
    internal class CreditsCommand
    {
        private readonly Config _config;
        private readonly UsersConnector _usersConnector;

        public CreditsCommand(Config config, UsersConnector usersConnector)
        {
            _config = config;
            _usersConnector = usersConnector;
        }

        [Command("credits")]
        [Aliases("cred")]
        [Description("Shows your current credits. This command has no arguments.")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        public async Task OnExecute(CommandContext command)
        {
            var currentCredits = await _usersConnector.CommandGetUserCredits(command.User.Id);

            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Info),
                Description = "You currently have:\n" + currentCredits + " credits"
            };
            await command.RespondAsync("", false, message);
        }
    }
}