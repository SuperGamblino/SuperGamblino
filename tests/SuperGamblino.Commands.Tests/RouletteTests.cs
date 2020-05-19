using SuperGamblino.Commands.Commands;
using SuperGamblino.Core.Helpers;
using SuperGamblino.Infrastructure.Connectors;

namespace SuperGamblino.Commands.Tests
{
    public class RouletteTests
    {
        private RouletteCommand GetRouletteCommandLogic(UsersConnector usersConnector,
            GameHistoryConnector gameHistoryConnector)
        {
            return new RouletteCommand(usersConnector, new BetSizeParser(), Helpers.GetMessages(),
                gameHistoryConnector);
        }

        //TODO Write unit tests
    }
}