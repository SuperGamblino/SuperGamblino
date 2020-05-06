using DSharpPlus.Entities;
using Moq;
using SuperGamblino.CommandsLogics;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;
using Xunit;

namespace SuperGamblinoTests.CommandsTests
{
    public class DailyRewardTests
    {
        private DailyRewardCommandLogic GetDailyRewardCommandLogic(UsersConnector usersConnector)
        {
            return new DailyRewardCommandLogic(usersConnector, Helpers.GetMessages());
        }

        [Fact]
        public async void InformAboutProblemsWithDb()
        {
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetDateTime(0, It.IsAny<string>()))
                .ReturnsAsync(new DateTimeResult(false, null));
            var logic = GetDailyRewardCommandLogic(usersConnector.Object);

            var result = await logic.GetDailyReward(0);

            Assert.Equal(new DiscordColor(Helpers.WarningColor), result.Color);
            Assert.Equal("Some problem with DB occured!", result.Description);
        }

        //TODO write more unit tests
    }
}