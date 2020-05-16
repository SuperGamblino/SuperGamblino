using System;
using Moq;
using SuperGamblino.Core.Entities;
using SuperGamblino.Infrastructure.Connectors;
using SuperGamblino.Infrastructure.DatabasesObjects;
using Xunit;

namespace SuperGamblino.Commands.Tests
{
    public class WorkRewardTests
    {
        private WorkRewardCommandLogic GetWorkRewardCommandLogic(UsersConnector usersConnector)
        {
            return new WorkRewardCommandLogic(Helpers.GetMessages(), usersConnector);
        }

        [Fact]
        public async void GiveRewardOnFirstUser()
        {
            var amountOfCredits = 0;
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetDateTime(0, It.IsAny<string>()))
                .ReturnsAsync(new DateTimeResult(true, null));
            usersConnector.Setup(x => x.GetUser(0))
                .ReturnsAsync(new User
                {
                    Id = 0,
                    Credits = 250,
                    Experience = 1200,
                    LastDailyReward = null,
                    LastHourlyReward = null,
                    Level = 6
                });
            usersConnector.Setup(x => x.CommandGiveCredits(0, It.IsAny<int>()))
                .Callback((ulong user, int credits) => { amountOfCredits += credits; });
            usersConnector.Setup(x => x.SetDateTime(0, It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(true);
            var logic = GetWorkRewardCommandLogic(usersConnector.Object);

            var result = await logic.GetWorkReward(0);

            Assert.Equal(Helpers.SuccessColor, result.Color);
            Assert.Equal("WorkReward", result.Title);
            Assert.Equal($"You've gained {amountOfCredits} credits!", result.Description);
            Assert.NotEqual(0, amountOfCredits);
        }

        [Fact]
        public async void GiveRewardOnLaterUse()
        {
            var amountOfCredits = 0;
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetDateTime(0, It.IsAny<string>()))
                .ReturnsAsync(new DateTimeResult(true, new DateTime(2020, 1, 1)));
            usersConnector.Setup(x => x.GetUser(0))
                .ReturnsAsync(new User
                {
                    Id = 0,
                    Credits = 250,
                    Experience = 1200,
                    LastDailyReward = null,
                    LastHourlyReward = null,
                    Level = 6
                });
            usersConnector.Setup(x => x.CommandGiveCredits(0, It.IsAny<int>()))
                .Callback((ulong user, int credits) => { amountOfCredits += credits; });
            usersConnector.Setup(x => x.SetDateTime(0, It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(true);
            var logic = GetWorkRewardCommandLogic(usersConnector.Object);

            var result = await logic.GetWorkReward(0);

            Assert.Equal(Helpers.SuccessColor, result.Color);
            Assert.Equal("WorkReward", result.Title);
            Assert.Equal($"You've gained {amountOfCredits} credits!", result.Description);
            Assert.NotEqual(0, amountOfCredits);
        }

        [Fact]
        public async void InformAboutCallingCommandTooEarly()
        {
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetDateTime(0, It.IsAny<string>()))
                .ReturnsAsync(new DateTimeResult(true, DateTime.Now));
            usersConnector.Setup(x => x.GetUser(0))
                .ReturnsAsync(new User
                {
                    Id = 0,
                    Credits = 250,
                    Experience = 1200,
                    LastDailyReward = null,
                    LastHourlyReward = null,
                    Level = 6
                });
            var logic = GetWorkRewardCommandLogic(usersConnector.Object);

            var result = await logic.GetWorkReward(0);

            Assert.Equal(Helpers.WarningColor, result.Color);
            Assert.Equal("WorkReward", result.Title);
            Assert.Matches(
                "You've tried to execute command '!work' before it was ready! Command will be ready in ([^\\s]+)",
                result.Description);
        }

        [Fact]
        public async void InformAboutProblemsWithDb()
        {
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetDateTime(0, It.IsAny<string>()))
                .ReturnsAsync(new DateTimeResult(false, null));
            usersConnector.Setup(x => x.GetUser(0))
                .ReturnsAsync(new User
                {
                    Id = 0,
                    Credits = 250,
                    Experience = 1200,
                    LastDailyReward = null,
                    LastHourlyReward = null,
                    Level = 6
                });
            var logic = GetWorkRewardCommandLogic(usersConnector.Object);

            var result = await logic.GetWorkReward(0);

            Assert.Equal(Helpers.WarningColor, result.Color);
            Assert.Equal("Some problem with DB occured!", result.Description);
        }
    }
}