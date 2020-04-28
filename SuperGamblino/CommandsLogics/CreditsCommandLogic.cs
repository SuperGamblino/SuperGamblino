using System.Threading.Tasks;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.CommandsLogics
{
    public class CreditsCommandLogic
    {
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public CreditsCommandLogic(UsersConnector usersConnector, MessagesHelper messagesHelper)
        {
            _usersConnector = usersConnector;
            _messagesHelper = messagesHelper;
        }

        public async Task<DiscordEmbed> GetCurrentCreditStatus(ulong userId)
        {
            var credits = await _usersConnector.CommandGetUserCredits(userId);
            return _messagesHelper.AddCoinsBalanceInformation(_messagesHelper.Information(""), credits);
        }
    }
}