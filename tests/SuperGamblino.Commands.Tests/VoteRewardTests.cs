using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Moq;
using Moq.Protected;
using SuperGamblino.Commands.Commands;
using SuperGamblino.Core.Configuration;
using SuperGamblino.Core.Entities;
using SuperGamblino.Infrastructure.Connectors;
using Xunit;

namespace SuperGamblino.Commands.Tests
{
    public class VoteRewardTests
    {
        private VoteRewardCommand GetVoteRewardCommandLogic(HttpMessageHandler httpMessageHandler,
            UsersConnector usersConnector, Config config = null)
        {
            return new VoteRewardCommand(new HttpClient(httpMessageHandler), usersConnector, config ?? Helpers.GetConfig(),
                Helpers.GetMessages(), Helpers.GetLogger<VoteRewardCommand>());
        }

        private HttpMessageHandler GetHttpClient(bool voted, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var httpMessageHandler = Helpers.GetHttpMessageHandler();
            httpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent("{\n  \"voted\" : " + (voted ? "true" : "false") + "\n}")
                });
            return httpMessageHandler.Object;
        }

        [Fact]
        public async void GetInfoThatUserHasNotVoted()
        {
            var httpMessageHandler = GetHttpClient(false);
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetUser(0)).ReturnsAsync(new User()
            {
                Id = 0,
                LastVoteReward = DateTime.Now.Subtract(TimeSpan.FromDays(5))
            });
            var logic = GetVoteRewardCommandLogic(httpMessageHandler, usersConnector.Object);

            var result = await logic.Vote(0);

            Assert.Equal(
                "To gain a vote reward, you have to use this link\n[Vote](https://top.gg/bot/688160933574475800/vote)",
                result.Description);
            Assert.Equal(Helpers.InfoColor, result.Color);
            Assert.Equal("You haven't voted yet!", result.Title);
        }

        [Fact]
        public async void GetInfoWhenBotHasNoTogGgToken()
        {
            var httpMessageHandler = GetHttpClient(false, HttpStatusCode.Unauthorized);
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetUser(0)).ReturnsAsync(new User()
            {
                Id = 0,
                LastVoteReward = DateTime.Now.Subtract(TimeSpan.FromDays(5))
            });
            var logic = GetVoteRewardCommandLogic(httpMessageHandler, usersConnector.Object);

            var result = await logic.Vote(0);

            Assert.Equal("Invalid Top GG Token Provided by the bot administrator! Please contact him!",
                result.Description);
            Assert.Equal(Helpers.WarningColor, result.Color);
        }

        [Fact]
        public async void GetInfoWhenCommandCalledTooEarly()
        {
            var httpMessageHandler = GetHttpClient(true);
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetUser(0)).ReturnsAsync(new User()
            {
                Id = 0,
                LastVoteReward = DateTime.Now
            });
            var logic = GetVoteRewardCommandLogic(httpMessageHandler, usersConnector.Object);

            var result = await logic.Vote(0);

            Assert.Equal(Helpers.WarningColor, result.Color);
            Assert.Equal("TopGGVote", result.Title);
            Assert.Matches(
                "You've tried to execute command '!vote' before it was ready! Command will be ready in ([^\\s]+)",
                result.Description);
        }

        [Fact]
        public async void GiveCreditsOnLaterVote()
        {
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetUser(0)).ReturnsAsync(new User()
            {
                Id = 0,
                LastVoteReward = DateTime.Now.Subtract(TimeSpan.FromDays(5))
            });
            usersConnector.Setup(x => x.GiveCredits(0, It.IsAny<int>()));
            usersConnector.Setup(x => x.UpdateUser(It.IsAny<User>()));
            var logic = GetVoteRewardCommandLogic(GetHttpClient(true), usersConnector.Object);

            var result = await logic.Vote(0);

            Assert.Matches("You've gained ([^\\s]+) credits!", result.Description);
            Assert.Equal(Helpers.SuccessColor, result.Color);
            Assert.Equal("TopGGVote", result.Title);
        }

        [Fact]
        public async void GivesCreditsOnFirstVote()
        {
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetUser(0)).ReturnsAsync(new User()
            {
                Id = 0,
                LastVoteReward = null
            });
            usersConnector.Setup(x => x.GiveCredits(0, It.IsAny<int>()));
            usersConnector.Setup(x => x.UpdateUser(It.IsAny<User>()));
            var logic = GetVoteRewardCommandLogic(GetHttpClient(true), usersConnector.Object);

            var result = await logic.Vote(0);

            Assert.Matches("You've gained ([^\\s]+) credits!", result.Description);
            Assert.Equal(Helpers.SuccessColor, result.Color);
            Assert.Equal("TopGGVote", result.Title);
        }

        [Fact]
        public async void GetInfoWhenTopGgFunctionalityIsTurnedOff()
        {
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            var config = Helpers.GetConfig();
            config.BotSettings.TopGgToken = null;
            var logic = GetVoteRewardCommandLogic(GetHttpClient(false), usersConnector.Object, config);

            var result = await logic.Vote(0);
            
            Assert.Equal("TopGGVote", result.Title);
            Assert.Equal(Helpers.InfoColor, result.Color);
            Assert.Equal("Top GG Token functionality is disabled on this server :disappointed:. Contact bots admin to turn it on :slight_smile:.",
                result.Description);
        }

        //sample of httpclient mocking => https://stackoverflow.com/questions/57091410/unable-to-mock-httpclient-postasync-in-unit-tests
    }
}