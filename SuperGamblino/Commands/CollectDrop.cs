using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.CommandsLogics;

namespace SuperGamblino.Commands
{
    internal class CollectDrop
    {
        private readonly CollectDropCommandLogic _logic;

        public CollectDrop(CollectDropCommandLogic logic)
        {
            _logic = logic;
        }

        [Command("collect")]
        [Cooldown(1, 2, CooldownBucketType.User)]
        [Description("Collects the reward from a coindrop.")]
        public async Task OnExecute(CommandContext command)
        {
            var arguments = command.RawArgumentString;
            await command.RespondAsync("", false,
                await _logic.Collect(arguments, command.Channel.Id, command.User.Id, command.User.Username));
        }
    }
}