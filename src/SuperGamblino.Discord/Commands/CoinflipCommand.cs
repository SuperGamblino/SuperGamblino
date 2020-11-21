using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace SuperGamblino.Discord.Commands
{
    internal class CoinflipCommand : BaseCommandModule
    {
        private readonly SuperGamblino.Commands.Commands.CoinflipCommand _logic;

        public CoinflipCommand(SuperGamblino.Commands.Commands.CoinflipCommand logic)
        {
            _logic = logic;
        }

        [Command("coinflip")]
        [Aliases("cf")]
        [Cooldown(1, 2, CooldownBucketType.User)]
        [Description(
            "Simple coinflip game. This command takes two argument Head or Tail and your bet. \nEx. coinflip head 100")]
        public async Task OnExecute(CommandContext command, string option, int bet)
        {
            var arguments = command.RawArgumentString;
            var userId = command.User.Id;
            var message = await command.RespondAsync("", false,
                await _logic.PlayCoinflip(arguments, userId).ToDiscordEmbed());
        }
    }
}