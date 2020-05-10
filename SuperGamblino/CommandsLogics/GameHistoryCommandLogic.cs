using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.CommandsLogics
{
    public class GameHistoryCommandLogic
    {
        private readonly GameHistoryConnector _gameHistoryConnector;
        private readonly MessagesHelper _messagesHelper;

        public GameHistoryCommandLogic(GameHistoryConnector gameHistoryConnector, MessagesHelper messagesHelper)
        {
            _gameHistoryConnector = gameHistoryConnector;
            _messagesHelper = messagesHelper;
        }

        public async Task<DiscordEmbed> GetGameHistory(ulong userId)
        {
            var history = await _gameHistoryConnector.GetGameHistories(userId);
            var text = string.Join("\n", history
                .TakeLast(10).Select(x => $"{x.GameName} | {(x.HasWon ? "Won" : "Lost")} | {x.CoinsDifference}")
                .ToArray());
            return _messagesHelper.Information(text, "GameHistory");
        }
    }
}