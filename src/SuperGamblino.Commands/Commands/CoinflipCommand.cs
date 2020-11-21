using System;
using System.Threading.Tasks;
using SuperGamblino.Core.Entities;
using SuperGamblino.Core.Helpers;
using SuperGamblino.Infrastructure.Connectors;
using SuperGamblino.Messages;

namespace SuperGamblino.Commands.Commands
{
    public class CoinflipCommand
    {
        private readonly BetSizeParser _betSizeParser;
        private readonly GameHistoryConnector _gameHistoryConnector;
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public CoinflipCommand(UsersConnector usersConnector, BetSizeParser betSizeParser,
            GameHistoryConnector gameHistoryConnector, MessagesHelper messagesHelper)
        {
            _usersConnector = usersConnector;
            _betSizeParser = betSizeParser;
            _gameHistoryConnector = gameHistoryConnector;
            _messagesHelper = messagesHelper;
        }

        public async Task<Message> PlayCoinflip(string argument, ulong userId)
        {
            var arguments = argument.ToUpper().TrimStart().Split(' ');

            if (arguments.Length != 2)
                return _messagesHelper.InvalidArguments(new[] {"<Head|Tail>", "<Bet>"}, "CoinFlip");

            var option = arguments[0] == "HEAD" || arguments[0] == "TAIL" ? arguments[0] : "";
            var amount = arguments[1].Trim();

            if (string.IsNullOrWhiteSpace(option) || string.IsNullOrWhiteSpace(amount))
                return _messagesHelper.InvalidArguments(new[] {"<Head|Tail>", "<Bet>"}, "CoinFlip");

            var bet = amount switch
            {
                "ALL" => await _usersConnector.GetCredits(userId),
                "HALF" => await _usersConnector.GetCredits(userId) / 2,
                _ => _betSizeParser.Parse(arguments[1])
            };

            if (bet == -1)
                return _messagesHelper.Error("Check your arguments (whether bet size does not equal 0 for example)!");

            if (await _usersConnector.TakeCredits(userId, bet))
            {
                var rnd = new Random();
                var hasWon = (Convert.ToBoolean(rnd.Next(0, 2)) ? "HEAD" : "TAIL") == option;

                var exp = ExpHelpers.CalculateBet((await _usersConnector.GetUser(userId)).Level, bet);
                var expResult = await _usersConnector.CommandGiveUserExp(userId, exp);

                if (hasWon) await _usersConnector.GiveCredits(userId, bet * 2);

                var embed = hasWon
                    ? _messagesHelper.WinInformation(bet, title: "CoinFlip")
                    : _messagesHelper.LoseInformation(bet, title: "CoinFlip");

                _messagesHelper.AddCoinsBalanceInformation(embed, await _usersConnector.GetCredits(userId));
                if (expResult.DidUserLevelUp) embed = _messagesHelper.AddLevelUpMessage(embed);

                await _gameHistoryConnector.AddGameHistory(new GameHistory
                {
                    GameName = "coinflip",
                    HasWon = hasWon,
                    CoinsDifference = hasWon ? bet : bet * -1,
                    UserId = userId
                });

                return embed;
            }

            return _messagesHelper.NotEnoughCredits("CoinFlip");
        }
    }
}