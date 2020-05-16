using SuperGamblino.Core.Helpers;
using SuperGamblino.Infrastructure.Connectors;

namespace SuperGamblino.Commands.Tests
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