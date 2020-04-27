using SuperGamblino.CommandsLogics;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblinoTests.CommandsTests
{
    public class HourlyRewardTests
    {
        private HourlyRewardCommandLogic GetHourlyRewardCommandLogic(UsersConnector usersConnector)
        {
            return new HourlyRewardCommandLogic(usersConnector, Helpers.GetMessages());
        }

        //TODO Write unit tests
    }
}