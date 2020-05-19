using System.Threading.Tasks;
using SuperGamblino.Infrastructure.Connectors;
using SuperGamblino.Messages;

namespace SuperGamblino.Commands.Commands
{
    public class CreditsCommand
    {
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public CreditsCommand(UsersConnector usersConnector, MessagesHelper messagesHelper)
        {
            _usersConnector = usersConnector;
            _messagesHelper = messagesHelper;
        }

        public async Task<Message> GetCurrentCreditStatus(ulong userId)
        {
            var credits = await _usersConnector.GetCredits(userId);
            return _messagesHelper.AddCoinsBalanceInformation(_messagesHelper.Information(""), credits);
        }
    }
}