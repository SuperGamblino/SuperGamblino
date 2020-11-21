using System.Threading.Tasks;
using SuperGamblino.Core.Entities;
using SuperGamblino.Core.GamesObjects;
using SuperGamblino.Core.Helpers;
using SuperGamblino.Infrastructure.Connectors;
using SuperGamblino.Messages;

namespace SuperGamblino.Commands.Commands
{
    public class SlotsCommand
    {
        private readonly BetSizeParser _betSizeParser;
        private readonly GameHistoryConnector _gameHistoryConnector;
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public SlotsCommand(UsersConnector usersConnector, BetSizeParser betSizeParser,
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
                    "ALL" => await _usersConnector.GetCredits(userId),
                    "HALF" => await _usersConnector.GetCredits(userId) / 2,
                    _ => _betSizeParser.Parse(argument[0])
                };
                if (bet == -1) return _messagesHelper.InvalidArguments(new[] {"slots <Bet>"}, "!slots", "Slots");
                if (await _usersConnector.TakeCredits(userId, bet))
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
                        await _usersConnector.GiveCredits(userId, pointsResult + bet);
                    }

                    var exp = ExpHelpers.CalculateBet((await _usersConnector.GetUser(userId)).Level, bet);
                    var expResult = await _usersConnector.CommandGiveUserExp(userId, exp);

                    await _gameHistoryConnector.AddGameHistory(new GameHistory
                    {
                        GameName = "slots",
                        HasWon = hasWon,
                        CoinsDifference = hasWon ? pointsResult : bet * -1,
                        UserId = userId
                    });
                    return _messagesHelper.AddCoinsBalanceAndExpInformation(hasWon
                            ? _messagesHelper.Success(
                                "You've won!\nResult: " + result.EmojiOne + " " + result.EmojiTwo + " " +
                                result.EmojiThree
                                + message, "Slots")
                            : _messagesHelper.LoseInformation(bet, "Slots"), expResult,
                        await _usersConnector.GetCredits(userId));
                }

                return _messagesHelper.NotEnoughCredits("Slots");
            }

            return _messagesHelper.InvalidArguments(new[] {"slots <Bet>"}, "!slots", "Slots");
        }
    }
}