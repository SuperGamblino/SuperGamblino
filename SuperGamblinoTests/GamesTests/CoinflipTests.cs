using System.Linq;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using SuperGamblino;
using SuperGamblino.CommandsLogics;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;
using SuperGamblino.Helpers;
using Xunit;
using ILogger = Castle.Core.Logging.ILogger;

namespace SuperGamblinoTests.GamesTests
{
    public class CoinflipTests
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

        private BetSizeParser GetBetSizeParser()
        {
            return new BetSizeParser();
        }

        private Mock<GameHistoryConnector> GetGameHistoryConnector()
        {
            return new Mock<GameHistoryConnector>(GetLogger(), GetConnectionString());
        }

        private Mock<UsersConnector> GetUsersConnector()
        {
            return new Mock<UsersConnector>(GetLogger(), GetConnectionString());
        }

        private Messages GetMessages()
        {
            return new Messages(GetConfig(), GetUsersConnector().Object);
        }

        private CoinflipCommandLogic GetCoinflipCommandLogic(UsersConnector usersConnector, GameHistoryConnector gameHistoryConnector)
        {
            return new CoinflipCommandLogic(usersConnector, GetBetSizeParser(), gameHistoryConnector, GetMessages());
        }
        
        [Theory]
        [InlineData("head 10 free")]
        [InlineData("head")]
        public async void DoesReturnInfoOnIncorrectArguments(string arguments)
        {
            //Arrange
            var usersConnector = GetUsersConnector();
            var gameHistoryConnector = GetGameHistoryConnector();
            var logic = GetCoinflipCommandLogic(usersConnector.Object, gameHistoryConnector.Object);
            
            //Assert
            var result = await logic.PlayCoinflip(arguments, 0);
            Assert.Contains("Invalid arguments!", result.Description);
            Assert.Equal(new DiscordColor("#bf1004"), result.Color);
        }

        [Theory]
        [InlineData("head 0")]
        [InlineData("head -10")]
        public async void DoesReturnErrorOnInvalidBetSize(string arguments)
        {
            //Arrange
            var usersConnector = GetUsersConnector();
            var gameHistoryConnector = GetGameHistoryConnector();
            var logic = GetCoinflipCommandLogic(usersConnector.Object, gameHistoryConnector.Object);
            
            //Assert
            var result = await logic.PlayCoinflip(arguments, 0);
            Assert.Contains("Error Occured!!!", result.Title);
            Assert.Contains("Check your arguments (whether bet size does not equal 0 for example)!", result.Description);
            Assert.Equal(new DiscordColor("#bf1004"), result.Color);
        }

        [Theory]
        [InlineData("head 10", 10)]
        [InlineData("tail 50", 50)]
        public async void DoesReturnCorrectAnswerOnLackOfCredits(string arguments, int amount)
        {
            var usersConnector = GetUsersConnector();
            usersConnector.Setup(x => x.CommandSubsctractCredits(0, amount)).ReturnsAsync(false);
            var gameHistoryConnector = GetGameHistoryConnector();
            var logic = GetCoinflipCommandLogic(usersConnector.Object, gameHistoryConnector.Object);

            var result = await logic.PlayCoinflip(arguments, 0);
            Assert.Contains("This is a casino, not a bank!\nYou do not have enough credits!", result.Description);
            Assert.Equal(new DiscordColor("#bf1004"), result.Color);
        }

        [Theory]
        [InlineData("head 10", 10)]
        [InlineData("tail 5000", 5000)]
        public async void DoesPlayingWorks(string arguments, int amount)
        {
            var usersConnector = GetUsersConnector();
            usersConnector.Setup(x => x.CommandSubsctractCredits(0, amount)).ReturnsAsync(true);
            usersConnector.Setup(x => x.CommandGiveCredits(0, amount * 2)).ReturnsAsync(0);
            usersConnector.Setup(x => x.CommandGetUserCredits(0)).ReturnsAsync(1000);
            usersConnector.Setup(x => x.GetUser(0)).ReturnsAsync(new User()
            {
                Level = 10
            });
            usersConnector.Setup(x => x.CommandGiveUserExp(0, It.IsAny<int>()))
                .ReturnsAsync(new AddExpResult(false, 100000, 0, 0));
            var gameHistoryConnector = GetGameHistoryConnector();
            var logic = GetCoinflipCommandLogic(usersConnector.Object, gameHistoryConnector.Object);

            var result = await logic.PlayCoinflip(arguments, 0);
            Assert.Matches("^You've (won|lost) ([^\\s]+) credits!$", result.Description);
            Assert.Equal("CoinFlip", result.Title);
        }
    }
}