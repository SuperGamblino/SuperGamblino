using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.CommandsLogics;

namespace SuperGamblino.Commands
{
    public class SlotsCommand
    {
        private readonly SlotsCommandLogic _logic;

        public SlotsCommand(SlotsCommandLogic logic)
        {
            _logic = logic;
        }

        [Command("slots")]
        [Aliases("slot")]
        [Cooldown(1, 2, CooldownBucketType.User)]
        [Description("Basic slots command!")]
        public async Task OnExecute(CommandContext command)
        {
            var arguments = command.RawArgumentString;
            await command.RespondAsync("", false, await _logic.PlaySlots(command.User.Id, arguments));
        }
    }
}