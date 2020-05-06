using System;
using DSharpPlus.Entities;
using Moq;
using SuperGamblino.CommandsLogics;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;
using Xunit;

namespace SuperGamblinoTests.CommandsTests
{
    public class CooldownTests
    {
        private CooldownCommandLogic GetCooldownCommandLogic(UsersConnector usersConnector)
        {
            return new CooldownCommandLogic(usersConnector, Helpers.GetMessages());
        }

        [Theory]
        [InlineData("last_hourly_reward")]
        [InlineData("last_daily_reward")]
        [InlineData("last_work_reward")]
        [InlineData("last_vote_reward")]
        public async void ReportsProblemWhenCannotGetValueFromDb(string fieldName)
        {
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetDateTime(0, It.IsAny<string>()))
                .ReturnsAsync((ulong id, string s) =>
                    s != fieldName ? new DateTimeResult(true, null) : new DateTimeResult(false, null));

            var logic = GetCooldownCommandLogic(usersConnector.Object);

            var result = await logic.GetCooldowns(0);

            Assert.Equal(new DiscordColor(Helpers.WarningColor), result.Color);
            Assert.Equal("Some problem with DB occured!!!", result.Description);
        }

        [Fact]
        public async void CorrectFormattingOfTheOutput()
        {
            var date = new DateTime(2020, 1, 1, 16, 0, 0);
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetDateTime(0, It.IsAny<string>()))
                .ReturnsAsync(new DateTimeResult(true, date));
            var logic = GetCooldownCommandLogic(usersConnector.Object);

            var result = await logic.GetCooldowns(0);

            Assert.Equal(new DiscordColor(Helpers.InfoColor), result.Color);
            Assert.Matches("Hourly : ([^\\s]+)\nDaily : ([^\\s]+)\nWork : ([^\\s]+)\nVote : ([^\\s]+)",
                result.Description);
        }

        //TODO Write more tests
    }
}