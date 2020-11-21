using Moq;
using SuperGamblino.Commands.Commands;
using SuperGamblino.Core.Entities;
using SuperGamblino.Infrastructure.Connectors;
using Xunit;

namespace SuperGamblino.Commands.Tests
{
    public class ShowProfileTests
    {
        private ShowProfileCommand GetShowProfileCommandLogic(UsersConnector usersConnector)
        {
            return new ShowProfileCommand(usersConnector, Helpers.GetMessages());
        }

        [Fact]
        public async void DoesProfileIsFormattedCorrectly()
        {
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetUser(0)).ReturnsAsync(new User
            {
                Credits = 100,
                Experience = 250,
                Id = 0,
                LastDailyReward = null,
                LastHourlyReward = null,
                Level = 4
            });
            var logic = GetShowProfileCommandLogic(usersConnector.Object);

            var result = await logic.ShowProfile(0, "S@mpleUser");

            Assert.Equal(Helpers.InfoColor, result.Color);
            Assert.Equal("User profile - S@mpleUser", result.Title);
            Assert.Equal("**Credits: ** 100\n" +
                         "**Level: ** 4\n" +
                         "**Current exp: ** 250\n" +
                         "**Job title: ** Super Market Assistant\n" +
                         "**Job salary: ** 45\n" +
                         "**Job cooldown: ** 6\n" +
                         "**Minimum bet: ** 60", result.Description);
        }
    }
}