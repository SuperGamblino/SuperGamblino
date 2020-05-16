using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.Commands;

namespace SuperGamblino.Discord.Commands
{
    internal class GamesHistoryCommand : BaseCommandModule
    {
        private readonly GameHistoryCommandLogic _logic;

        public GamesHistoryCommand(GameHistoryCommandLogic logic)
        {
            _logic = logic;
        }

        [Command("history")]
        [Cooldown(1, 2, CooldownBucketType.User)]
        [Description("Displays the 10 recent games and the results. This command takes no arguments.")]
        public async Task OnExecute(CommandContext command)
        {
            await command.RespondAsync("", false, await _logic.GetGameHistory(command.User.Id).ToDiscordEmbed());
        }
    }
}