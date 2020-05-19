using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.Commands;
using SuperGamblino.Commands.Commands;

namespace SuperGamblino.Discord.Commands
{
    internal class GlobalTopCommand : BaseCommandModule
    {
        private readonly SuperGamblino.Commands.Commands.GlobalTopCommand _logic;

        public GlobalTopCommand(SuperGamblino.Commands.Commands.GlobalTopCommand logic)
        {
            _logic = logic;
        }

        [Command("globaltop")]
        [Aliases("gt")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [Description("Shows a global leaderboard based on credits. This command takes no arguments.")]
        public async Task OnExecute(CommandContext command)
        {
            await command.RespondAsync("", false,
                await _logic.GetGlobalTop(async id => (await command.Client.GetUserAsync(id)).Username).ToDiscordEmbed());
        }
    }
}