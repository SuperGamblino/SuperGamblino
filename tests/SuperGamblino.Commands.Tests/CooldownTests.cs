using System;
using System.Linq;
using DSharpPlus.Entities;
using Moq;
using SuperGamblino.Infrastructure.Connectors;
using SuperGamblino.Infrastructure.DatabasesObjects;
using Xunit;

namespace SuperGamblino.Commands.Tests
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

            Assert.Equal(Helpers.WarningColor, result.Color);
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

            Assert.Equal(Helpers.InfoColor, result.Color);
            Assert.Matches("Hourly : ([^\\s]+)\nDaily : ([^\\s]+)\nWork : ([^\\s]+)\nVote : ([^\\s]+)",
                result.Description);
        }

        [Fact]
        public async void CorrectValuesInTheOutput()
        {
            var date = DateTime.Now;
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetDateTime(0, It.IsAny<string>()))
                .ReturnsAsync(new DateTimeResult(true, date));
            var logic = GetCooldownCommandLogic(usersConnector.Object);

            var result = await logic.GetCooldowns(0);
            var times = result.Description.Split('\n').SkipLast(1) //converts output result to TimeSpan[]
                .Select(x => string.Join(':', x.Split(':').Skip(1)))
                .Select(x => TimeSpan.Parse(x))
                .ToArray();

            Assert.Equal(Helpers.InfoColor, result.Color);
            Assert.Equal(4, times.Length);
            Assert.Equal(TimeSpan.FromHours(1).Subtract(TimeSpan.FromSeconds(1)), times[0]);
            Assert.Equal(TimeSpan.FromDays(1).Subtract(TimeSpan.FromSeconds(1)), times[1]);
            Assert.Equal(TimeSpan.FromHours(6).Subtract(TimeSpan.FromSeconds(1)), times[2]);
            Assert.Equal(TimeSpan.FromHours(12).Subtract(TimeSpan.FromSeconds(1)), times[3]);
        }
    }
}