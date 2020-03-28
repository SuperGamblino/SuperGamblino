using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.GameObjects;
using static SuperGamblino.GameObjects.Work;

namespace SuperGamblino.Commands
{
    internal class ShowProfile
    {
        private readonly Database _database;
        private readonly Messages _messages;

        public ShowProfile(Database database, Messages messages)
        {
            _database = database;
            _messages = messages;
        }

        [Command("profile")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [Description("Shows a profile, descriping detailed information about the user. This command has no arguments.")]
        public async Task OnExecute(CommandContext command)
        {
            User user = await _database.GetUser(command);
            Job currentJob = GetCurrentJob(user.Level);
            await _messages.Profile(command, user, currentJob);
        }
    }
}