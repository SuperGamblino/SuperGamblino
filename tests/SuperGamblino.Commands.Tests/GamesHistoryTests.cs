using DSharpPlus.Entities;
using Moq;
using SuperGamblino.Core.CommandsObjects;
using SuperGamblino.Infrastructure.Connectors;
using Xunit;

namespace SuperGamblino.Commands.Tests
{
    public class GamesHistoryTests
    {
        private GameHistoryCommandLogic GetGameHistoryCommandLogic(GameHistoryConnector gameHistoryConnector)
        {
            return new GameHistoryCommandLogic(gameHistoryConnector, Helpers.GetMessages());
        }

        [Fact]
        public async void DoGetGameHistoryWorks()
        {
            var gameHistoryConnector = Helpers.GetDatabaseConnector<GameHistoryConnector>();
            gameHistoryConnector.Setup(x => x.GetGameHistories(0)).ReturnsAsync(new[]
            {
                new GameHistory {CoinsDifference = 100, GameName = "Sample Game", HasWon = true},
                new GameHistory {CoinsDifference = -200, GameName = "Sample Another Game", HasWon = false}
            });
            var logic = GetGameHistoryCommandLogic(gameHistoryConnector.Object);

            var result = await logic.GetGameHistory(0);

            Assert.Equal(Helpers.InfoColor, result.Color);
            Assert.Equal("GameHistory", result.Title);
            Assert.Equal("Sample Game | Won | 100\nSample Another Game | Lost | -200", result.Description);
        }
    }
}