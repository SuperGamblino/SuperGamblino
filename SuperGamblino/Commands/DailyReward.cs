using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace SuperGamblino.Commands
{
    public class DailyReward
    {

        [Command("daily")]
        [Aliases("get-daily")]
        [Cooldown(1, 86400, CooldownBucketType.User)]
        public async Task OnExecute(CommandContext command)
        {
            const int reward = 500;
            Database.CommandGiveCredits(command.User.Id, reward);
            await Messages.CoinsGain(command, reward);
        }
    }
}