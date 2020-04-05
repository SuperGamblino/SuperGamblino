using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.Commands
{
    internal class GamesHistory
    {
        private readonly GameHistoryConnector _gameHistoryConnector;
        private readonly Config _config;

        public GamesHistory(Config config, GameHistoryConnector gameHistoryConnector)
        {
            _config = config;
            _gameHistoryConnector = gameHistoryConnector;
        }

        [Command("history")]
        [Cooldown(1,2,CooldownBucketType.User)]
        [Description("Displays the 10 recent games and the results. This command takes no arguments.")]
        public async Task OnExecute(CommandContext command)
        {
            var history = await _gameHistoryConnector.GetGameHistories(command.User.Id);
            var text = string.Join("\n", history.GameHistories
                .TakeLast(10).Select(x => $"{x.GameName} | {(x.HasWon ? "Won" : "Lost")} | {x.CoinsDifference}").ToArray());
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