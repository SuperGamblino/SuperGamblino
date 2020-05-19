using System.Collections.Generic;
using Moq;
using SuperGamblino.Commands.Commands;
using SuperGamblino.Core.Entities;
using SuperGamblino.Infrastructure.Connectors;
using Xunit;

namespace SuperGamblino.Commands.Tests
{
    public class GlobalTopTests
    {
        private GlobalTopCommand GetGlobalTopCommandLogic(UsersConnector usersConnector)
        {
            return new GlobalTopCommand(usersConnector, Helpers.GetLogger<GlobalTopCommand>(),
                Helpers.GetMessages());
        }

        [Fact]
        public async void CorrectlyFormatOutput()
        {
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.CommandGetGlobalTop()).ReturnsAsync(new List<User>
            {
                new User
                {
                    Credits = 1200,
                    Experience = 100,
                    Id = 1,
                    LastDailyReward = null,
                    LastHourlyReward = null,
                    Level = 3
                },
                new User
                {
                    Credits = 100,
                    Experience = 0,
                    Id = 0,
                    LastDailyReward = null,
                    LastHourlyReward = null,
                    Level = 1
                }
            });
            var logic = GetGlobalTopCommandLogic(usersConnector.Object);

#pragma warning disable 1998
            var result = await logic.GetGlobalTop(async id =>
#pragma warning restore 1998
            {
                return id switch
                {
                    0 => "SampleUser1",
                    1 => "SampleUser2",
                    _ => "SampleUser3"
                };
            });

            Assert.Equal(Helpers.InfoColor, result.Color);
            Assert.Equal("GlobalTop", result.Title);
            Assert.Equal("SampleUser2: 1200\nSampleUser1: 100\n", result.Description);
        }

        //TODO Add more unit tests
        [Fact]
        public async void HandleBrokenGetUsernameMethod()
        {
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.CommandGetGlobalTop()).ReturnsAsync(new List<User>
            {
                new User
                {
                    Credits = 100,
                    Experience = 0,
                    Id = 0,
                    LastDailyReward = null,
                    LastHourlyReward = null,
                    Level = 1
                }
            });
            var logic = GetGlobalTopCommandLogic(usersConnector.Object);

            var result = await logic.GetGlobalTop(null);

            Assert.Equal(Helpers.InfoColor, result.Color);
            Assert.Equal("GlobalTop", result.Title);
            Assert.Equal("", result.Description);
        }
    }
}