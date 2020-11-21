using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace SuperGamblino.Discord.Commands
{
    internal class WorkRewardCommand : BaseCommandModule
    {
        private readonly SuperGamblino.Commands.Commands.WorkRewardCommand _logic;

        public WorkRewardCommand(SuperGamblino.Commands.Commands.WorkRewardCommand logic)
        {
            _logic = logic;
        }

        [Command("work")]
        [Aliases("get-work")]
        [Description("Gives you credits based on your job. This command has no arguments.")]
        public async Task OnExecute(CommandContext command)
        {
            await command.RespondAsync("", false, await _logic.GetWorkReward(command.User.Id).ToDiscordEmbed());
        }
    }
}