using DSharpPlus.Entities;
using Moq;
using SuperGamblino.CommandsLogics;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;
using SuperGamblino.Helpers;
using Xunit;

namespace SuperGamblinoTests.CommandsTests
{
    public class CoinflipTests
    {
        private CoinflipCommandLogic GetCoinflipCommandLogic(UsersConnector usersConnector,
            GameHistoryConnector gameHistoryConnector)
        {
            return new CoinflipCommandLogic(usersConnector, new BetSizeParser(), gameHistoryConnector,
                Helpers.GetMessages());
        }

        [Theory]
        [InlineData("head 10 free")]
        [InlineData("head")]
        public async void DoesReturnInfoOnIncorrectArguments(string arguments)
        {
            //Arrange
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            var gameHistoryConnector = Helpers.GetDatabaseConnector<GameHistoryConnector>();
            var logic = GetCoinflipCommandLogic(usersConnector.Object, gameHistoryConnector.Object);

            //Assert
            var result = await logic.PlayCoinflip(arguments, 0);
            Assert.Contains("Invalid arguments!", result.Description);
            Assert.Equal(new DiscordColor(Helpers.WarningColor), result.Color);
        }

        [Theory]
        [InlineData("head 0")]
        [InlineData("head -10")]
        public async void DoesReturnErrorOnInvalidBetSize(string arguments)
        {
            //Arrange
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            var gameHistoryConnector = Helpers.GetDatabaseConnector<GameHistoryConnector>();
            var logic = GetCoinflipCommandLogic(usersConnector.Object, gameHistoryConnector.Object);

            //Assert
            var result = await logic.PlayCoinflip(arguments, 0);
            Assert.Contains("Error Occured!!!", result.Title);
            Assert.Contains("Check your arguments (whether bet size does not equal 0 for example)!",
                result.Description);
            Assert.Equal(new DiscordColor(Helpers.WarningColor), result.Color);
        }

        [Theory]
        [InlineData("head 10", 10)]
        [InlineData("tail 50", 50)]
        public async void DoesReturnCorrectAnswerOnLackOfCredits(string arguments, int amount)
        {
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.CommandSubsctractCredits(0, amount)).ReturnsAsync(false);
            var gameHistoryConnector = Helpers.GetDatabaseConnector<GameHistoryConnector>();
            var logic = GetCoinflipCommandLogic(usersConnector.Object, gameHistoryConnector.Object);

            var result = await logic.PlayCoinflip(arguments, 0);
            Assert.Contains("This is a casino, not a bank!\nYou do not have enough credits!", result.Description);
            Assert.Equal(new DiscordColor(Helpers.WarningColor), result.Color);
        }

        [Theory]
        [InlineData("head 10", 10)]
        [InlineData("tail 5000", 5000)]
        public async void DoesPlayingWorks(string arguments, int amount)
        {
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.CommandSubsctractCredits(0, amount)).ReturnsAsync(true);
            usersConnector.Setup(x => x.CommandGiveCredits(0, amount * 2)).ReturnsAsync(0);
            usersConnector.Setup(x => x.CommandGetUserCredits(0)).ReturnsAsync(1000);
            usersConnector.Setup(x => x.GetUser(0)).ReturnsAsync(new User
            {
                Level = 10
            });
            usersConnector.Setup(x => x.CommandGiveUserExp(0, It.IsAny<int>()))
                .ReturnsAsync(new AddExpResult(false, 100000, 0, 0));
            var gameHistoryConnector = Helpers.GetDatabaseConnector<GameHistoryConnector>();
            gameHistoryConnector.Setup(x => x.AddGameHistory(0, It.IsAny<GameHistory>()))
                .ReturnsAsync(true);
            var logic = GetCoinflipCommandLogic(usersConnector.Object, gameHistoryConnector.Object);

            var result = await logic.PlayCoinflip(arguments, 0);
            Assert.Matches("^You've (won|lost) ([^\\s]+) credits!$", result.Description);
            Assert.Equal("CoinFlip", result.Title);
        }
    }
}