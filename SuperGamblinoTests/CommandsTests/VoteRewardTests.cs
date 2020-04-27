using System.Net.Http;
using SuperGamblino.CommandsLogics;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblinoTests.CommandsTests
{
    public class VoteRewardTests
    {
        private VoteRewardCommandLogic GetVoteRewardCommandLogic(HttpMessageHandler httpMessageHandler,
            UsersConnector usersConnector)
        {
            return new VoteRewardCommandLogic(new HttpClient(httpMessageHandler), usersConnector, Helpers.GetConfig(),
                Helpers.GetMessages());
        }

        //TODO Write unit tests (sample of httpclient mocking => https://stackoverflow.com/questions/57091410/unable-to-mock-httpclient-postasync-in-unit-tests)
    }
}