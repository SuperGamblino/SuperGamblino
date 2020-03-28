using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace SuperGamblino.Commands
{
    internal class GlobalTopCommand
    {
        private readonly Config _config;
        private readonly Database _database;

        public GlobalTopCommand(Database database, Config config)
        {
            _database = database;
            _config = config;
        }

        [Command("globaltop")]
        [Aliases("gt")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [Description("Shows a global leaderboard based on credits. This command takes no arguments.")]
        public async Task OnExecute(CommandContext command)
        {
            var listUsers = await _database.CommandGetGlobalTop(command);

            var desc = "";
            foreach (var user in listUsers) desc += user.DiscordUser.Username + ": " + user.Credits + "\n";

            var message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(_config.ColorSettings.Info),
                Title = "Top 10 users",
                Description = desc
            };
            await command.RespondAsync("", false, message);
        }
    }
}