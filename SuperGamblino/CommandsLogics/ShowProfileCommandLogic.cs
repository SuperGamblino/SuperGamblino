using System.Threading.Tasks;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;

namespace SuperGamblino.CommandsLogics
{
    public class ShowProfileCommandLogic
    {
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public ShowProfileCommandLogic(UsersConnector usersConnector, MessagesHelper messagesHelper)
        {
            _usersConnector = usersConnector;
            _messagesHelper = messagesHelper;
        }

        public async Task<DiscordEmbed> ShowProfile(ulong userId, string username)
        {
            var user = await _usersConnector.GetUser(userId);
            var currentJob = Work.GetCurrentJob(user.Level);
            return _messagesHelper.GetProfile(user, currentJob, username);
        }
    }
}