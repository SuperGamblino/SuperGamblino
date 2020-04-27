using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.CommandsLogics;

namespace SuperGamblino.Commands
{
    internal class DailyReward
    {
        private readonly DailyRewardCommandLogic _logic;

        public DailyReward(DailyRewardCommandLogic logic)
        {
            _logic = logic;
        }

        //Reward size can be adjusted in DailyRewardCommandLogic class

        [Command("daily")]
        [Aliases("get-daily")]
        [Description("Gives you daily credits. This command is available once every day.")]
        public async Task OnExecute(CommandContext command)
        {
            await command.RespondAsync("", false, await _logic.GetDailyReward(command.User.Id));
        }
    }
}