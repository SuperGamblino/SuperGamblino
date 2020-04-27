using DSharpPlus.Entities;
using Moq;
using SuperGamblino.CommandsLogics;
using SuperGamblino.DatabaseConnectors;
using Xunit;

namespace SuperGamblinoTests.CommandsTests
{
    public class CoinDropCollectingTests
    {
        private CollectDropCommandLogic GetCollectDropCommandLogic(CoindropConnector coindropConnector,
            UsersConnector usersConnector)
        {
            return new CollectDropCommandLogic(Helpers.GetMessages(), coindropConnector, usersConnector);
        }

        [Theory]
        [InlineData("")]
        [InlineData("100 100")]
        [InlineData("test")]
        public async void DoesReturnInfoOnIncorrectArguments(string argument)
        {
            var coinDropConnector = Helpers.GetDatabaseConnector<CoindropConnector>();
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            var logic = GetCollectDropCommandLogic(coinDropConnector.Object, usersConnector.Object);

            var result = await logic.Collect(argument, 0, 0, "test-user");
            Assert.Equal(new DiscordColor(Helpers.WarningColor), result.Color);
            Assert.Contains("Invalid arguments!", result.Description);
            Assert.Equal("Collect", result.Title);
        }

        [Fact]
        public async void DoesCollectingWork()
        {
            var earned = 0;
            var coinDropConnector = Helpers.GetDatabaseConnector<CoindropConnector>();
            coinDropConnector.Setup(x => x.CollectCoinDrop(0, 4453)).ReturnsAsync(100);
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.CommandGiveCredits(0, It.IsAny<int>())).ReturnsAsync((ulong uId, int y) =>
            {
                earned += y;
                return earned;
            });
            var logic = GetCollectDropCommandLogic(coinDropConnector.Object, usersConnector.Object);

            var result = await logic.Collect("4453", 0, 0, "test-user");

            Assert.Equal(new DiscordColor(Helpers.SuccessColor), result.Color);
            Assert.Contains("Congratulations, the CoinDrop has been collected!\n**Reward**", result.Description);
            Assert.Equal("CoinDropCollected", result.Title);
            Assert.Contains("Collected by", result.Footer.Text);
            Assert.Equal(100, earned);
        }

        [Fact]
        public async void DoesInformationWhenDropIsCollectedWork()
        {
            var coinDropConnector = Helpers.GetDatabaseConnector<CoindropConnector>();
            coinDropConnector.Setup(x => x.CollectCoinDrop(0, 4453)).ReturnsAsync(0);
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            var logic = GetCollectDropCommandLogic(coinDropConnector.Object, usersConnector.Object);

            var result = await logic.Collect("4453", 0, 0, "test-user");

            Assert.Equal(new DiscordColor(Helpers.InfoColor), result.Color);
            Assert.Contains("Sadly the CoinDrop has already been collected!", result.Description);
            Assert.Equal("CoinDropAlreadyCollected", result.Title);
        }
    }
}