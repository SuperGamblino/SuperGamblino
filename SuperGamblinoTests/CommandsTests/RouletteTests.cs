using SuperGamblino.CommandsLogics;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.Helpers;

namespace SuperGamblinoTests.CommandsTests
{
    public class RouletteTests
    {
        private RouletteCommandLogic GetRouletteCommandLogic(UsersConnector usersConnector,
            GameHistoryConnector gameHistoryConnector)
        {
            return new RouletteCommandLogic(usersConnector, new BetSizeParser(), Helpers.GetMessages(),
                gameHistoryConnector);
        }

        //TODO Write unit tests
    }
}