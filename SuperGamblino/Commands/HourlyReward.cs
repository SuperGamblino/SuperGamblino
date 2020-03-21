using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace SuperGamblino.Commands
{
    public class HourlyReward
    {
        [Command("hourly")]
        [Aliases("get-hourly")]
        [Cooldown(1, 3600, CooldownBucketType.User)]
        public async Task OnExecute(CommandContext command)
        {
            const int reward = 20;
            Database.CommandGiveCredits(command.User.Id, reward);
            await Messages.CoinsGain(command, reward);
        }
    }
}