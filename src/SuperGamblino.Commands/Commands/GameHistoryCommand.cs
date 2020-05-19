using System.Linq;
using System.Threading.Tasks;
using SuperGamblino.Infrastructure.Connectors;
using SuperGamblino.Messages;

namespace SuperGamblino.Commands.Commands
{
    public class GameHistoryCommand
    {
        private readonly GameHistoryConnector _gameHistoryConnector;
        private readonly MessagesHelper _messagesHelper;

        public GameHistoryCommand(GameHistoryConnector gameHistoryConnector, MessagesHelper messagesHelper)
        {
            _gameHistoryConnector = gameHistoryConnector;
            _messagesHelper = messagesHelper;
        }

        public async Task<Message> GetGameHistory(ulong userId)
        {
            var history = await _gameHistoryConnector.GetGameHistories(userId);
            var text = string.Join("\n", history
                .TakeLast(10).Select(x => $"{x.GameName} | {(x.HasWon ? "Won" : "Lost")} | {x.CoinsDifference}")
                .ToArray());
            return _messagesHelper.Information(text, "GameHistory");
        }
    }
}