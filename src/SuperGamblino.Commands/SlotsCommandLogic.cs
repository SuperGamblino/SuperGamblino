using System.Threading.Tasks;
using SuperGamblino.Core.CommandsObjects;
using SuperGamblino.Core.GamesObjects;
using SuperGamblino.Core.Helpers;
using SuperGamblino.Infrastructure.Connectors;
using SuperGamblino.Infrastructure.DatabasesObjects;
using SuperGamblino.Messages;

namespace SuperGamblino.Commands
{
    public class SlotsCommandLogic
    {
        private readonly BetSizeParser _betSizeParser;
        private readonly GameHistoryConnector _gameHistoryConnector;
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public SlotsCommandLogic(UsersConnector usersConnector, BetSizeParser betSizeParser,
            GameHistoryConnector gameHistoryConnector, MessagesHelper messagesHelper)
        {
            _usersConnector = usersConnector;
            _betSizeParser = betSizeParser;
            _gameHistoryConnector = gameHistoryConnector;
            _messagesHelper = messagesHelper;
        }

        public async Task<Message> PlaySlots(ulong userId, string arguments)
        {
            var argument = arguments.ToUpper().TrimStart().Split(' ');
            if (argument.Length == 1)
            {
                var bet = argument[0].Trim() switch
                {
                    "ALL" => await _usersConnector.CommandGetUserCredits(userId),
                    "HALF" => await _usersConnector.CommandGetUserCredits(userId) / 2,
                    _ => _betSizeParser.Parse(argument[0])
                };
                if (bet == -1) return _messagesHelper.InvalidArguments(new[] {"slots <Bet>"}, "!slots", "Slots");
                if (await _usersConnector.CommandSubsctractCredits(userId, bet))
                {
                    var hasWon = false;

                    var result = Slots.GetResult();

                    var pointsResult = Slots.GetPointsFromResult(result, bet);

                    var message = "";

                    if (pointsResult > 0)
                    {
                        hasWon = true;
                        if (Slots.IsJackpot(result))
                            message = "\n" + "JACKPOT!!! You won " + pointsResult + " points!";
                        else if (Slots.IsDouble(result))
                            message = "\n" + "DOUBLE! You won " + pointsResult + " points!";
                        await _usersConnector.CommandGiveCredits(userId, pointsResult + bet);
                    }

                    var expHelper = new Exp(_usersConnector);
                    var expResult = await expHelper.Give(userId, bet);
                    await _gameHistoryConnector.AddGameHistory(userId, new GameHistory
                    {
                        GameName = "slots",
                        HasWon = hasWon,
                        CoinsDifference = hasWon ? pointsResult : bet * -1
                    });
                    return _messagesHelper.AddCoinsBalanceAndExpInformation(hasWon
                            ? _messagesHelper.Success(
                                "You've won!\nResult: " + result.EmojiOne + " " + result.EmojiTwo + " " +
                                result.EmojiThree
                                + message, "Roulette")
                            : _messagesHelper.LoseInformation(bet, "Roulette"), expResult,
                        await _usersConnector.CommandGetUserCredits(userId));
                }

                return _messagesHelper.NotEnoughCredits("Roulette");
            }

            return _messagesHelper.InvalidArguments(new[] {"slots <Bet>"}, "!slots", "Slots");
        }
    }
}