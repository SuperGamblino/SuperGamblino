using SuperGamblino.Core.Helpers;
using SuperGamblino.Infrastructure.Connectors;

namespace SuperGamblino.Commands.Tests
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