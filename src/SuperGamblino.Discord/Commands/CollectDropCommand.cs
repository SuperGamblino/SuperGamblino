using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.Commands;
using SuperGamblino.Commands.Commands;

namespace SuperGamblino.Discord.Commands
{
    internal class CollectDropCommand : BaseCommandModule
    {
        private readonly SuperGamblino.Commands.Commands.CollectDropCommand _logic;

        public CollectDropCommand(SuperGamblino.Commands.Commands.CollectDropCommand logic)
        {
            _logic = logic;
        }

        [Command("collect")]
        [Cooldown(1, 2, CooldownBucketType.User)]
        [Description("Collects the reward from a coindrop.")]
        public async Task OnExecute(CommandContext command, int id)
        {
            var arguments = command.RawArgumentString;
            await command.RespondAsync("", false,
                await _logic.Collect(arguments, command.Channel.Id, command.User.Id, command.User.Username).ToDiscordEmbed());
        }
    }
}