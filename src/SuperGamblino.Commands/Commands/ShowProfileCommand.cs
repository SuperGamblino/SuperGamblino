using System.Threading.Tasks;
using SuperGamblino.Core.GamesObjects;
using SuperGamblino.Infrastructure.Connectors;
using SuperGamblino.Messages;

namespace SuperGamblino.Commands.Commands
{
    public class ShowProfileCommand
    {
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public ShowProfileCommand(UsersConnector usersConnector, MessagesHelper messagesHelper)
        {
            _usersConnector = usersConnector;
            _messagesHelper = messagesHelper;
        }

        public async Task<Message> ShowProfile(ulong userId, string username)
        {
            var user = await _usersConnector.GetUser(userId);
            var currentJob = Work.GetCurrentJob(user.Level);
            return _messagesHelper.GetProfile(user, currentJob, username);
        }
    }
}