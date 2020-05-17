using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.Commands;
using SuperGamblino.Commands.Commands;

namespace SuperGamblino.Discord.Commands
{
    internal class RouletteCommand : BaseCommandModule
    {
        private readonly SuperGamblino.Commands.Commands.RouletteCommand _logic;

        public RouletteCommand(SuperGamblino.Commands.Commands.RouletteCommand logic)
        {
            _logic = logic;
        }

        [Command("roulette")]
        [Cooldown(1, 2, CooldownBucketType.User)]
        [Description("<Red|Black|Odd|Even|Number> <Bet>\n\nEx. roulette Red 100")]
        public async Task OnExecute(CommandContext command, string options, int bet)
        {
            var arguments = command.RawArgumentString;
            await command.RespondAsync("", false, await _logic.PlayRoulette(command.User.Id, arguments).ToDiscordEmbed());
        }
    }
}