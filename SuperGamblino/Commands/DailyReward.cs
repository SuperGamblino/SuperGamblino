using System;
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
        public async Task OnExecute(CommandContext command)
        {
            const int reward = 500;
            var result = await Database.GetDateTime(command.User.Id, "last_daily_reward");
            if (result.Successful)
            {
                if (result.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - result.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromDays(1))
                    {
                        await Database.CommandGiveCredits(command.User.Id, reward);
                        await Database.SetDateTime(command.User.Id, "last_daily_reward", DateTime.Now);
                        await Messages.CoinsGain(command, reward);
                    }
                    else
                    {
                        await Messages.TooEarly(command, TimeSpan.FromDays(1) - timeSpan);
                    }
                }
                else
                {
                    await Database.CommandGiveCredits(command.User.Id, reward);
                    await Database.SetDateTime(command.User.Id, "last_daily_reward", DateTime.Now);
                    await Messages.CoinsGain(command, reward);
                }
            }
            else
            {
                await Messages.Error(command, "Some problem with DB occured!");
            }
        }
    }
}