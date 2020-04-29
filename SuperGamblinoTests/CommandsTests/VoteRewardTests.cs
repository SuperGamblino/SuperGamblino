using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Moq;
using Moq.Protected;
using SuperGamblino.CommandsLogics;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;
using Xunit;

namespace SuperGamblinoTests.CommandsTests
{
    public class VoteRewardTests
    {
        private VoteRewardCommandLogic GetVoteRewardCommandLogic(HttpMessageHandler httpMessageHandler,
            UsersConnector usersConnector)
        {
            return new VoteRewardCommandLogic(new HttpClient(httpMessageHandler), usersConnector, Helpers.GetConfig(),
                Helpers.GetMessages(), Helpers.GetLogger<VoteRewardTests>());
        }

        [Fact]
        public async void GetsErrorOnDBError()
        {
            var httpMessageHandler = Helpers.GetHttpMessageHandler();
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetDateTime(0, "last_vote_reward"))
                .ReturnsAsync(new DateTimeResult(false, null));
            var logic = GetVoteRewardCommandLogic(httpMessageHandler.Object, usersConnector.Object);

            var result = await logic.Vote(0);
            
            Assert.Equal(new DiscordColor(Helpers.WarningColor), result.Color);
            Assert.Equal("Some problem with DB occured!", result.Description);
        }

        [Fact]
        public async void GetInfoWhenBotHasNoTogGgToken()
        {
            var httpMessageHandler = Helpers.GetHttpMessageHandler();
            httpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage() {StatusCode = HttpStatusCode.Unauthorized});
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x => x.GetDateTime(0, "last_vote_reward"))
                .ReturnsAsync(new DateTimeResult(true, DateTime.Now.Subtract(TimeSpan.FromDays(5))));
            var logic = GetVoteRewardCommandLogic(httpMessageHandler.Object, usersConnector.Object);

            var result = await logic.Vote(0);
            
            Assert.Equal(new DiscordColor(Helpers.WarningColor), result.Color);
            Assert.Equal("Invalid Top GG Token Provided by the bot administrator! Please contact him!", result.Description);
        }

        [Fact]
        public async void GetInfoThatUserHasNotVoted()
        {
            var httpMessageHandler = Helpers.GetHttpMessageHandler();
            httpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                    {StatusCode = HttpStatusCode.OK, Content = new StringContent("{\n  \"voted\" : false\n}")});
            var usersConnector = Helpers.GetDatabaseConnector<UsersConnector>();
            usersConnector.Setup(x=>x.GetDateTime(0, "last_vote_reward"))
                .ReturnsAsync(new DateTimeResult(true, DateTime.Now.Subtract(TimeSpan.FromDays(5))));
            var logic = GetVoteRewardCommandLogic(httpMessageHandler.Object, usersConnector.Object);

            var result = await logic.Vote(0);
            
            Assert.Equal(new DiscordColor(Helpers.InfoColor), result.Color);
            Assert.Equal("To gain a vote reward, you have to use this link\n[Vote](https://top.gg/bot/688160933574475800/vote)", result.Description);
            Assert.Equal("You haven't voted yet!", result.Title);
        }
        //TODO Write unit tests (sample of httpclient mocking => https://stackoverflow.com/questions/57091410/unable-to-mock-httpclient-postasync-in-unit-tests)
    }
}