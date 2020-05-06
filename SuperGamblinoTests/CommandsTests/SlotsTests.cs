using SuperGamblino.CommandsLogics;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.Helpers;

namespace SuperGamblinoTests.CommandsTests
{
    public class SlotsTests
    {
        private SlotsCommandLogic GetSlotsCommandLogic(UsersConnector usersConnector,
            GameHistoryConnector gameHistoryConnector)
        {
            return new SlotsCommandLogic(usersConnector, new BetSizeParser(), gameHistoryConnector,
                Helpers.GetMessages());
        }

        //TODO Write unit tests
    }
}