using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;
using SuperGamblino.Helpers;

namespace SuperGamblino.CommandsLogics
{
    public class RouletteCommandLogic
    {
        private readonly BetSizeParser _betSizeParser;
        private readonly GameHistoryConnector _gameHistoryConnector;
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public RouletteCommandLogic(UsersConnector usersConnector, BetSizeParser betSizeParser,
            MessagesHelper messagesHelper,
            GameHistoryConnector gameHistoryConnector)
        {
            _usersConnector = usersConnector;
            _betSizeParser = betSizeParser;
            _messagesHelper = messagesHelper;
            _gameHistoryConnector = gameHistoryConnector;
        }

        public async Task<DiscordEmbed> PlayRoulette(ulong userId, string arguments)
        {
            var argument = arguments != null
                ? arguments.ToUpper().TrimStart().Split(' ')
                : new string[0];
            var isNumber = false;
            var nmbBet = -1;
            var nmbGuess = 0;
            if (argument.Length >= 2)
            {
                isNumber = int.TryParse(argument[0], out nmbGuess);
                if (argument[1].Trim() == "ALL")
                    nmbBet = await _usersConnector.CommandGetUserCredits(userId);
                else if (argument[1].Trim() == "HALF")
                    nmbBet = await _usersConnector.CommandGetUserCredits(userId) / 2;
                else
                    nmbBet = _betSizeParser.Parse(argument[1]);
            }

            var credWon = 0;
            if (nmbBet != -1)
            {
                var curCred = await _usersConnector.CommandGetUserCredits(userId);
                if (curCred < nmbBet)
                    return _messagesHelper.NotEnoughCredits("Roulette");
                await _usersConnector.CommandSubsctractCredits(userId, nmbBet);

                var rewardMulti = 0;
                var hasWon = false;
                var random = new Random();
                var number = random.Next(0, 37);
                var expHelper = new Exp(_usersConnector);

                var expResult = await expHelper.Give(userId, nmbBet);
                var result = Roulette.GetResult(number);

                if (isNumber)
                {
                    if (nmbGuess == result.Number)
                    {
                        hasWon = true;
                        rewardMulti = 36;
                    }
                }
                else
                {
                    switch (argument[0].ToUpper())
                    {
                        case "RED":
                            if (result.Color.ToUpper() == "RED")
                            {
                                hasWon = true;
                                rewardMulti = 2;
                            }

                            break;
                        case "BLACK":
                            if (result.Color.ToUpper() == "BLACK")
                            {
                                hasWon = true;
                                rewardMulti = 2;
                            }

                            break;
                        case "ODD":
                            if (result.OddOrEven.ToUpper() == "ODD")
                            {
                                hasWon = true;
                                rewardMulti = 2;
                            }

                            break;
                        case "EVEN":
                            if (result.OddOrEven.ToUpper() == "EVEN")
                            {
                                hasWon = true;
                                rewardMulti = 2;
                            }

                            break;
                        default:
                            return _messagesHelper.InvalidArguments(new[] {"<Red|Black|Odd|Even|Number> <Bet>"},
                                "!roulette", "Roulette");
                    }
                }

                await _gameHistoryConnector.AddGameHistory(userId, new GameHistory
                {
                    GameName = "roulette",
                    HasWon = hasWon,
                    CoinsDifference = credWon
                });
                DiscordEmbedBuilder responseBuilder;
                if (hasWon)
                {
                    credWon = nmbBet * rewardMulti;
                    await _usersConnector.CommandGiveCredits(userId, credWon);
                    responseBuilder = _messagesHelper.WinInformation(credWon, "Roulette");
                }
                else
                {
                    responseBuilder = _messagesHelper.LoseInformation(nmbBet, "Roulette");
                }

                return _messagesHelper.AddCoinsBalanceAndExpInformation(responseBuilder, expResult,
                    await _usersConnector.CommandGetUserCredits(userId));
            }

            return _messagesHelper.InvalidArguments(new[] {"<Red|Black|Odd|Even|Number> <Bet>"}, "!roulette",
                "Roulette");
        }
    }
}