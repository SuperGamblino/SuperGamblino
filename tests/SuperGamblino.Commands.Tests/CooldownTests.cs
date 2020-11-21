using System;
using System.Linq;
using Moq;
using SuperGamblino.Commands.Commands;
using SuperGamblino.Core.Entities;
using SuperGamblino.Infrastructure.Connectors;
using Xunit;

namespace SuperGamblino.Commands.Tests
{
    public class CooldownTests
    {
        private CooldownCommand GetCooldownCommandLogic(UsersConnector usersConnector)
        {
            return new CooldownCommand(usersConnector, Helpers.GetMessages());
        }

        [Fact]
        public async void CorrectFormattingOfTheOutput()
        {
            var date = new DateTime(2020, 1, 1, 16, 0, 0);
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetUser(0)).ReturnsAsync(new User
            {
                Id = 0,
                LastDailyReward = DateTime.Now,
                LastHourlyReward = DateTime.Now,
                LastVoteReward = DateTime.Now,
                LastWorkReward = DateTime.Now
            });
            var logic = GetCooldownCommandLogic(usersConnector.Object);

            var result = await logic.GetCooldowns(0);

            Assert.Equal(Helpers.InfoColor, result.Color);
            Assert.Matches("Hourly : ([^\\s]+)\nDaily : ([^\\s]+)\nWork : ([^\\s]+)\nVote : ([^\\s]+)",
                result.Description);
        }

        [Fact]
        public async void CorrectValuesInTheOutput()
        {
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetUser(0)).ReturnsAsync(new User
            {
                Id = 0,
                LastDailyReward = DateTime.Now,
                LastHourlyReward = DateTime.Now,
                LastVoteReward = DateTime.Now,
                LastWorkReward = DateTime.Now
            });
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