using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.Commands;

namespace SuperGamblino.Discord.Commands
{
    public class SlotsCommand : BaseCommandModule
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
        public async Task OnExecute(CommandContext command, int bet)
        {
            var arguments = command.RawArgumentString;
            await command.RespondAsync("", false, await _logic.PlaySlots(command.User.Id, arguments).ToDiscordEmbed());
        }
    }
}