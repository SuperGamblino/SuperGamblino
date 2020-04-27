using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.CommandsLogics;

namespace SuperGamblino.Commands
{
    internal class HourlyReward
    {
        private readonly HourlyRewardCommandLogic _logic;

        public HourlyReward(HourlyRewardCommandLogic logic)
        {
            _logic = logic;
        }

        [Command("hourly")]
        [Aliases("get-hourly")]
        [Description("Gives you 20 credits. This command is available once every hour.")]
        public async Task OnExecute(CommandContext command)
        {
            await command.RespondAsync("", false, await _logic.GetHourlyReward(command.User.Id));
        }
    }
}