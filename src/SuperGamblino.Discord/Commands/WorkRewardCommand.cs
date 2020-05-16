using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.Commands;

namespace SuperGamblino.Discord.Commands
{
    internal class WorkRewardCommand : BaseCommandModule
    {
        private readonly WorkRewardCommandLogic _logic;

        public WorkRewardCommand(WorkRewardCommandLogic logic)
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