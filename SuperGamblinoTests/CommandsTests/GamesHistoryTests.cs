using SuperGamblino.CommandsLogics;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblinoTests.CommandsTests
{
    public class GamesHistoryTests
    {
        private GameHistoryCommandLogic GetGameHistoryCommandLogic(GameHistoryConnector gameHistoryConnector)
        {
            return new GameHistoryCommandLogic(gameHistoryConnector, Helpers.GetMessages());
        }

        //TODO Write unit tests
    }
}