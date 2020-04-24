using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using SuperGamblino;
using SuperGamblino.CommandsLogics;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.Helpers;
using Xunit;

namespace SuperGamblinoTests.GamesTests
{
    public class CoinDropCollectingTests
    {
        private ILogger<CoinflipTests> GetLogger()
        {
            return LoggerFactory.Create(x=>x.AddConsole()).CreateLogger<CoinflipTests>();
        }

        private Config GetConfig()
        {
            return new Config
            {
                DatabaseSettings = new DatabaseSettings()
                {
                    Address = "<Address>",
                    Name = "<Name>",
                    Password = "<Password>",
                    Port = 1122,
                    Username = "<Username>"
                },
                ColorSettings = new ColorSettings() {Info = "#439ff0", Success = "#4beb50", Warning = "#bf1004"}
            };
            ;
        }

        private ConnectionString GetConnectionString()
        {
            return new ConnectionString(GetConfig());
        }

        private Mock<CoindropConnector> GetCoindropConnector()
        {
            return new Mock<CoindropConnector>(GetLogger(), GetConnectionString());
        }

        private Mock<UsersConnector> GetUsersConnector()
        {
            return new Mock<UsersConnector>(GetLogger(), GetConnectionString());
        }

        private Messages GetMessages()
        {
            return new Messages(GetConfig(), GetUsersConnector().Object);
        }

        private CollectDropCommandLogic GetCollectDropCommandLogic(CoindropConnector coindropConnector, UsersConnector usersConnector)
        {
            return new CollectDropCommandLogic(GetMessages(), coindropConnector, usersConnector);
        }

        [Theory]
        [InlineData("")]
        [InlineData("100 100")]
        [InlineData("test")]
        public async void DoesReturnInfoOnIncorrectArguments(string argument)
        {
            var coinDropConnector = GetCoindropConnector();
            var usersConnector = GetUsersConnector();
            var logic = GetCollectDropCommandLogic(coinDropConnector.Object, usersConnector.Object);

            var result = await logic.Collect(argument, 0, 0, "test-user");
            Assert.Equal(new DiscordColor("#bf1004"), result.Color);
            Assert.Contains("Invalid arguments!", result.Description);
            Assert.Equal("Collect", result.Title);
        }

        [Fact]
        public async void DoesCollectingWork()
        {
            var earned = 0;
            var coinDropConnector = GetCoindropConnector();
            coinDropConnector.Setup(x => x.CollectCoinDrop(0, 4453)).ReturnsAsync(100);
            var usersConnector = GetUsersConnector();
            usersConnector.Setup(x => x.CommandGiveCredits(0, It.IsAny<int>())).ReturnsAsync((ulong uId, int y) =>
            {
                earned += y;
                return earned;
            });
            var logic = GetCollectDropCommandLogic(coinDropConnector.Object, usersConnector.Object);

            var result = await logic.Collect("4453", 0, 0, "test-user");
            
            Assert.Equal(new DiscordColor("#4beb50"), result.Color);
            Assert.Contains("Congratulations, the CoinDrop has been collected!\n**Reward**", result.Description);
            Assert.Equal("CoinDropCollected", result.Title);
            Assert.Contains("Collected by", result.Footer.Text);
            Assert.Equal(100, earned);
        }

        [Fact]
        public async void DoesInformationWhenDropIsCollectedWork()
        {
            var coinDropConnector = GetCoindropConnector();
            coinDropConnector.Setup(x => x.CollectCoinDrop(0, 4453)).ReturnsAsync(0);
            var usersConnector = GetUsersConnector();
            var logic = GetCollectDropCommandLogic(coinDropConnector.Object, usersConnector.Object);

            var result = await logic.Collect("4453", 0, 0, "test-user");
            
            Assert.Equal(new DiscordColor("#439ff0"), result.Color);
            Assert.Contains("Sadly the CoinDrop has already been collected!", result.Description);
            Assert.Equal("CoinDropAlreadyCollected", result.Title);
        }
    }
}