using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;
using static SuperGamblino.GameObjects.Work;

namespace SuperGamblino.Commands
{
    internal class ShowProfile
    {
        private readonly UsersConnector _usersConnector;
        private readonly Messages _messages;

        public ShowProfile(Messages messages, UsersConnector usersConnector)
        {
            _messages = messages;
            _usersConnector = usersConnector;
        }

        [Command("profile")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [Description("Shows a profile, descriping detailed information about the user. This command has no arguments.")]
        public async Task OnExecute(CommandContext command)
        {
            User user = await _usersConnector.GetUser(command.User.Id);
            Job currentJob = GetCurrentJob(user.Level);
            await _messages.Profile(command, user, currentJob);
        }
    }
}