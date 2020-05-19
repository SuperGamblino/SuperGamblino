using System;
using DSharpPlus.Entities;
using Moq;
using SuperGamblino.Commands.Commands;
using SuperGamblino.Core.Entities;
using SuperGamblino.Infrastructure.Connectors;
using Xunit;

namespace SuperGamblino.Commands.Tests
{
    public class HourlyRewardTests
    {
        private HourlyRewardCommand GetHourlyRewardCommandLogic(UsersConnector usersConnector)
        {
            return new HourlyRewardCommand(usersConnector, Helpers.GetMessages());
        }

        [Fact]
                public async void GiveRewardOnFirstUser()
                {
                    var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
                    usersConnector.Setup(x => x.GetUser(0)).ReturnsAsync(new User()
                    {
                        Id = 0,
                        LastDailyReward = null
                    });
                    usersConnector.Setup(x => x.UpdateUser(It.IsAny<User>()));
                    usersConnector.Setup(x => x.GiveCredits(0, It.IsAny<int>()));
                    var logic = GetHourlyRewardCommandLogic(usersConnector.Object);
        
                    var result = await logic.GetHourlyReward(0);
        
                    Assert.Equal(Helpers.SuccessColor, result.Color);
                    Assert.Equal("HourlyReward", result.Title);
                    Assert.Matches(@"You've gained ([^\s]+) credits!", result.Description);
                }
        
                [Fact]
                public async void GiveRewardOnLaterUse()
                {
                    var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
                    usersConnector.Setup(x => x.GetUser(0)).ReturnsAsync(new User()
                    {
                        Id = 0,
                        LastDailyReward = new DateTime(2020, 1, 1)
                    });
                    usersConnector.Setup(x => x.UpdateUser(It.IsAny<User>()));
                    usersConnector.Setup(x => x.GiveCredits(0, It.IsAny<int>()));
                    var logic = GetHourlyRewardCommandLogic(usersConnector.Object);
        
                    var result = await logic.GetHourlyReward(0);
        
                    Assert.Equal(Helpers.SuccessColor, result.Color);
                    Assert.Equal("HourlyReward", result.Title);
                    Assert.Matches(@"You've gained ([^\s]+) credits!", result.Description);
                }
        
                [Fact]
                public async void InformAboutCallingCommandTooEarly()
                {
                    var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
                    usersConnector.Setup(x => x.GetUser(0))
                        .ReturnsAsync(new User()
                        {
                            Id = 0,
                            LastHourlyReward = DateTime.Now
                        });
                    var logic = GetHourlyRewardCommandLogic(usersConnector.Object);
        
                    var result = await logic.GetHourlyReward(0);
        
                    Assert.Equal(Helpers.WarningColor, result.Color);
                    Assert.Equal("HourlyReward", result.Title);
                    Assert.Matches(
                        "You've tried to execute command '!hourly' before it was ready! Command will be ready in ([^\\s]+)",
                        result.Description);
                }
    }
}