using SuperGamblino.Commands.Commands;
using SuperGamblino.Core.Helpers;
using SuperGamblino.Infrastructure.Connectors;

namespace SuperGamblino.Commands.Tests
{
    public class SlotsTests
    {
        private SlotsCommand GetSlotsCommandLogic(UsersConnector usersConnector,
            GameHistoryConnector gameHistoryConnector)
        {
            return new SlotsCommand(usersConnector, new BetSizeParser(), gameHistoryConnector,
                Helpers.GetMessages());
        }

        //TODO Write unit tests
    }
}