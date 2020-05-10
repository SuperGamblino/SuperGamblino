using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;
using SuperGamblino.Helpers;

namespace SuperGamblino.CommandsLogics
{
    public class CoinflipCommandLogic
    {
        private readonly BetSizeParser _betSizeParser;
        private readonly GameHistoryConnector _gameHistoryConnector;
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public CoinflipCommandLogic(UsersConnector usersConnector, BetSizeParser betSizeParser,
            GameHistoryConnector gameHistoryConnector, MessagesHelper messagesHelper)
        {
            _usersConnector = usersConnector;
            _betSizeParser = betSizeParser;
            _gameHistoryConnector = gameHistoryConnector;
            _messagesHelper = messagesHelper;
        }

        public async Task<DiscordEmbed> PlayCoinflip(string argument, ulong userId)
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
                "ALL" => await _usersConnector.CommandGetUserCredits(userId),
                "HALF" => await _usersConnector.CommandGetUserCredits(userId) / 2,
                _ => _betSizeParser.Parse(arguments[1])
            };

            if (bet == -1)
                return _messagesHelper.Error("Check your arguments (whether bet size does not equal 0 for example)!");

            if (await _usersConnector.CommandSubsctractCredits(userId, bet))
            {
                var rnd = new Random();
                var hasWon = (Convert.ToBoolean(rnd.Next(0, 2)) ? "HEAD" : "TAIL") == option;

                var expHelper = new Exp(_usersConnector);
                var expResult = await expHelper.Give(userId, bet);

                if (hasWon) await _usersConnector.CommandGiveCredits(userId, bet * 2);

                var embed = hasWon
                    ? _messagesHelper.WinInformation(bet, "CoinFlip")
                    : _messagesHelper.LoseInformation(bet, "CoinFlip");

                _messagesHelper.AddCoinsBalanceInformation(embed, await _usersConnector.CommandGetUserCredits(userId));
                if (expResult.DidUserLevelUp) embed = _messagesHelper.AddLevelUpMessage(embed);

                await _gameHistoryConnector.AddGameHistory(userId, new GameHistory
                {
                    GameName = "coinflip",
                    HasWon = hasWon,
                    CoinsDifference = hasWon ? bet : bet * -1
                });

                return embed;
            }

            return _messagesHelper.NotEnoughCredits("CoinFlip");
        }
    }
}