using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.CommandsLogics;

namespace SuperGamblino.Commands
{
    internal class RouletteCommand
    {
        private readonly RouletteCommandLogic _logic;

        public RouletteCommand(RouletteCommandLogic logic)
        {
            _logic = logic;
        }

        [Command("roulette")]
        [Cooldown(1, 2, CooldownBucketType.User)]
        [Description("<Red|Black|Odd|Even|Number> <Bet>\n\nEx. roulette Red 100")]
        public async Task OnExecute(CommandContext command)
        {
            var arguments = command.RawArgumentString;
            await command.RespondAsync("", false, await _logic.PlayRoulette(command.User.Id, arguments));
        }
    }
}