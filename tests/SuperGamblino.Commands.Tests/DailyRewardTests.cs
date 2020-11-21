using System;
using Moq;
using SuperGamblino.Commands.Commands;
using SuperGamblino.Core.Entities;
using SuperGamblino.Infrastructure.Connectors;
using Xunit;

namespace SuperGamblino.Commands.Tests
{
    public class DailyRewardTests
    {
        private DailyRewardCommand GetDailyRewardCommandLogic(UsersConnector usersConnector)
        {
            return new DailyRewardCommand(usersConnector, Helpers.GetMessages());
        }

        [Fact]
        public async void GiveRewardOnFirstUser()
        {
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetUser(0)).ReturnsAsync(new User
            {
                Id = 0,
                LastDailyReward = null
            });
            usersConnector.Setup(x => x.UpdateUser(It.IsAny<User>()));
            usersConnector.Setup(x => x.GiveCredits(0, It.IsAny<int>()));
            var logic = GetDailyRewardCommandLogic(usersConnector.Object);

            var result = await logic.GetDailyReward(0);

            Assert.Equal(Helpers.SuccessColor, result.Color);
            Assert.Equal("DailyReward", result.Title);
            Assert.Matches(@"You've gained ([^\s]+) credits!", result.Description);
        }

        [Fact]
        public async void GiveRewardOnLaterUse()
        {
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetUser(0)).ReturnsAsync(new User
            {
                Id = 0,
                LastDailyReward = new DateTime(2020, 1, 1)
            });
            usersConnector.Setup(x => x.UpdateUser(It.IsAny<User>()));
            usersConnector.Setup(x => x.GiveCredits(0, It.IsAny<int>()));
            var logic = GetDailyRewardCommandLogic(usersConnector.Object);

            var result = await logic.GetDailyReward(0);

            Assert.Equal(Helpers.SuccessColor, result.Color);
            Assert.Equal("DailyReward", result.Title);
            Assert.Matches(@"You've gained ([^\s]+) credits!", result.Description);
        }

        [Fact]
        public async void InformAboutCallingCommandTooEarly()
        {
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetUser(0))
                .ReturnsAsync(new User
                {
                    Id = 0,
                    LastDailyReward = DateTime.Now
                });
            var logic = GetDailyRewardCommandLogic(usersConnector.Object);

            var result = await logic.GetDailyReward(0);

            Assert.Equal(Helpers.WarningColor, result.Color);
            Assert.Equal("DailyReward", result.Title);
            Assert.Matches(
                "You've tried to execute command '!daily' before it was ready! Command will be ready in ([^\\s]+)",
                result.Description);
        }
    }
}