using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;
using static SuperGamblino.GameObjects.Work;

namespace SuperGamblino.Commands
{
    internal class About
    {
        private readonly Messages _messages;

        public About(Messages messages)
        {
            _messages = messages;
        }

        [Command("about")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [Description("Shows how many server the bot has been added to.")]
        public async Task OnExecute(CommandContext command)
        {
            await _messages.About(command);
        }
    }
}