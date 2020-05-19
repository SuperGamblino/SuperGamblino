using System;
using System.Threading.Tasks;
using SuperGamblino.Core.CommandsObjects;
using SuperGamblino.Core.Entities;
using SuperGamblino.Core.GamesObjects;
using SuperGamblino.Core.Helpers;
using SuperGamblino.Infrastructure.Connectors;
using SuperGamblino.Messages;

namespace SuperGamblino.Commands.Commands
{
    public class RouletteCommand
    {
        private readonly BetSizeParser _betSizeParser;
        private readonly GameHistoryConnector _gameHistoryConnector;
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public RouletteCommand(UsersConnector usersConnector, BetSizeParser betSizeParser,
            MessagesHelper messagesHelper,
            GameHistoryConnector gameHistoryConnector)
        {
            _usersConnector = usersConnector;
            _betSizeParser = betSizeParser;
            _messagesHelper = messagesHelper;
            _gameHistoryConnector = gameHistoryConnector;
        }

        public async Task<Message> PlayRoulette(ulong userId, string arguments)
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
                    nmbBet = await _usersConnector.GetCredits(userId);
                else if (argument[1].Trim() == "HALF")
                    nmbBet = await _usersConnector.GetCredits(userId) / 2;
                else
                    nmbBet = _betSizeParser.Parse(argument[1]);
            }

            var selectedOption = argument[0].ToUpper();

            var credWon = 0;
            if (nmbBet != -1)
            {
                var curCred = await _usersConnector.GetCredits(userId);
                if (curCred < nmbBet)
                    return _messagesHelper.NotEnoughCredits("Roulette");
                await _usersConnector.TakeCredits(userId, nmbBet);

                var rewardMulti = 0;
                var hasWon = false;
                var random = new Random();
                var number = random.Next(0, 37);
                var exp = ExpHelpers.CalculateBet((await _usersConnector.GetUser(userId)).Level, nmbBet);
                var expResult = await _usersConnector.CommandGiveUserExp(userId, exp);
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
                    switch (selectedOption)
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

                await _gameHistoryConnector.AddGameHistory(new GameHistory
                {
                    GameName = "roulette",
                    HasWon = hasWon,
                    CoinsDifference = credWon,
                    UserId = userId
                });
                Message responseBuilder;
                if (hasWon)
                {
                    credWon = nmbBet * rewardMulti;
                    await _usersConnector.GiveCredits(userId, credWon);
                    responseBuilder = _messagesHelper.WinInformation(credWon,$"Result: {result.OddOrEven} {result.Color} {result.Number} | Your bet: {selectedOption}","Roulette");
                }
                else
                {
                    responseBuilder = _messagesHelper.LoseInformation(nmbBet,$"Result: {result.OddOrEven} {result.Color} {result.Number} | Your bet: {selectedOption}", "Roulette");
                }

                return _messagesHelper.AddCoinsBalanceAndExpInformation(responseBuilder, expResult,
                    await _usersConnector.GetCredits(userId));
            }

            return _messagesHelper.InvalidArguments(new[] {"<Red|Black|Odd|Even|Number> <Bet>"}, "!roulette",
                "Roulette");
        }
    }
}