using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.Commands;

namespace SuperGamblino.Discord.Commands
{
    internal class HourlyRewardCommand : BaseCommandModule
    {
        private readonly HourlyRewardCommandLogic _logic;

        public HourlyRewardCommand(HourlyRewardCommandLogic logic)
        {
            _logic = logic;
        }

        [Command("hourly")]
        [Aliases("get-hourly")]
        [Description("Gives you 20 credits. This command is available once every hour.")]
        public async Task OnExecute(CommandContext command)
        {
            await command.RespondAsync("", false, await _logic.GetHourlyReward(command.User.Id).ToDiscordEmbed());
        }
    }
}