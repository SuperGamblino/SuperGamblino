using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace SuperGamblino.Commands
{
    internal class GamesHistory
    {
        private readonly Database _database;
        private readonly Config _config;

        public GamesHistory(Database database, Config config)
        {
            _database = database;
            _config = config;
        }

        [Command("history")]
        [Cooldown(1,2,CooldownBucketType.User)]
        [Description("Displays the recent games and the results here from. This command takes no arguments.")]
        public async Task OnExecute(CommandContext command)
        {
            var history = await _database.GetGameHistories(command.User.Id);
            var text = string.Join("\n", history.GameHistories
                .Select(x => $"{x.GameName} | {(x.HasWon ? "Won" : "Lost")} | {x.CoinsDifference}").ToArray());
            var message = new DiscordEmbedBuilder()
            {
                Color = new DiscordColor(_config.ColorSettings.Info),
                Title = "Games history",
                Description = $"Game name | Has the game been won? | Coins difference\n{text}" 
            };
            await command.RespondAsync("", false, message.Build());
        }
    }
}