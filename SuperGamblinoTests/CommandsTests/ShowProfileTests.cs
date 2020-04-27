using SuperGamblino.CommandsLogics;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblinoTests.CommandsTests
{
    public class ShowProfileTests
    {
        private ShowProfileCommandLogic GetShowProfileCommandLogic(UsersConnector usersConnector)
        {
            return new ShowProfileCommandLogic(usersConnector, Helpers.GetMessages());
        }

        //TODO Write unit tests
    }
}