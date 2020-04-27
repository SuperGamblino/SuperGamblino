using SuperGamblino.CommandsLogics;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblinoTests.CommandsTests
{
    public class WorkRewardTests
    {
        private WorkRewardCommandLogic GetWorkRewardCommandLogic(UsersConnector usersConnector)
        {
            return new WorkRewardCommandLogic(Helpers.GetMessages(), usersConnector);
        }

        //TODO Write unit tests
    }
}