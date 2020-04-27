using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.CommandsLogics
{
    public class GameHistoryCommandLogic
    {
        private readonly GameHistoryConnector _gameHistoryConnector;
        private readonly Messages _messages;

        public GameHistoryCommandLogic(GameHistoryConnector gameHistoryConnector, Messages messages)
        {
            _gameHistoryConnector = gameHistoryConnector;
            _messages = messages;
        }

        public async Task<DiscordEmbed> GetGameHistory(ulong userId)
        {
            var history = await _gameHistoryConnector.GetGameHistories(userId);
            var text = string.Join("\n", history.GameHistories
                .TakeLast(10).Select(x => $"{x.GameName} | {(x.HasWon ? "Won" : "Lost")} | {x.CoinsDifference}")
                .ToArray());
            return _messages.Information(text, "GameHistory");
        }
    }
}