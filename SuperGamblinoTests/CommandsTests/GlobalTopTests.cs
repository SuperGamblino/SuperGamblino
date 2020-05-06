using SuperGamblino.CommandsLogics;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblinoTests.CommandsTests
{
    public class GlobalTopTests
    {
        private GlobalTopCommandLogic GetGlobalTopCommandLogic(UsersConnector usersConnector)
        {
            return new GlobalTopCommandLogic(usersConnector, Helpers.GetLogger<GlobalTopTests>(),
                Helpers.GetMessages());
        }

        //TODO Write unit tests
    }
}