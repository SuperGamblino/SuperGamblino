using System.Threading.Tasks;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;

namespace SuperGamblino.CommandsLogics
{
    public class ShowProfileCommandLogic
    {
        private readonly Messages _messages;
        private readonly UsersConnector _usersConnector;

        public ShowProfileCommandLogic(UsersConnector usersConnector, Messages messages)
        {
            _usersConnector = usersConnector;
            _messages = messages;
        }

        public async Task<DiscordEmbed> ShowProfile(ulong userId, string username)
        {
            var user = await _usersConnector.GetUser(userId);
            var currentJob = Work.GetCurrentJob(user.Level);
            return _messages.GetProfile(user, currentJob, username);
        }
    }
}