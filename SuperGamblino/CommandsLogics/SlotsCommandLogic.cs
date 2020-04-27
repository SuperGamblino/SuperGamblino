using System.Threading.Tasks;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;
using SuperGamblino.Helpers;

namespace SuperGamblino.CommandsLogics
{
    public class SlotsCommandLogic
    {
        private readonly BetSizeParser _betSizeParser;
        private readonly GameHistoryConnector _gameHistoryConnector;
        private readonly Messages _messages;
        private readonly UsersConnector _usersConnector;

        public SlotsCommandLogic(UsersConnector usersConnector, BetSizeParser betSizeParser,
            GameHistoryConnector gameHistoryConnector, Messages messages)
        {
            _usersConnector = usersConnector;
            _betSizeParser = betSizeParser;
            _gameHistoryConnector = gameHistoryConnector;
            _messages = messages;
        }

        public async Task<DiscordEmbed> PlaySlots(ulong userId, string arguments)
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
                if (bet == -1) return _messages.InvalidArguments(new[] {"slots <Bet>"}, "!slots", "Slots");
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
                    return _messages.AddCoinsBalanceAndExpInformation(hasWon
                            ? _messages.Success(
                                "You've won!\nResult: " + result.EmojiOne + " " + result.EmojiTwo + " " +
                                result.EmojiThree
                                + message, "Roulette")
                            : _messages.LoseInformation(bet, "Roulette"), expResult,
                        await _usersConnector.CommandGetUserCredits(userId));
                }

                return _messages.NotEnoughCredits("Roulette");
            }

            return _messages.InvalidArguments(new[] {"slots <Bet>"}, "!slots", "Slots");
        }
    }
}