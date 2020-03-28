using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace SuperGamblino.Commands
{
    internal class CreditsCommand
    {
        private readonly Config _config;
        private readonly Database _database;

        public CreditsCommand(Config config, Database database)
        {
            _config = config;
            _database = database;
        }

        [Command("credits")]
        [Aliases("cred")]
        [Description("Shows your current credits. This command has no arguments.")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        public async Task OnExecute(CommandContext command)
        {
            var currentCredits = await _database.CommandGetUserCredits(command.User.Id);

            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Info),
                Description = "You currently have:\n" + currentCredits + " credits"
            };
            await command.RespondAsync("", false, message);
        }
    }
}