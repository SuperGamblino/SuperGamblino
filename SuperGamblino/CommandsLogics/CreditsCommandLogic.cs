using System.Threading.Tasks;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.CommandsLogics
{
    public class CreditsCommandLogic
    {
        private readonly Messages _messages;
        private readonly UsersConnector _usersConnector;

        public CreditsCommandLogic(UsersConnector usersConnector, Messages messages)
        {
            _usersConnector = usersConnector;
            _messages = messages;
        }

        public async Task<DiscordEmbed> GetCurrentCreditStatus(ulong userId)
        {
            var credits = await _usersConnector.CommandGetUserCredits(userId);
            return _messages.AddCoinsBalanceInformation(_messages.Information(""), credits);
        }
    }
}