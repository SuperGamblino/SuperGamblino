using System;
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
        public async Task OnExecute(CommandContext command)
        {
            const int reward = 20;
            var result = await Database.GetDateTime(command.User.Id, "last_hourly_reward");
            if (result.Successful)
            {
                if (result.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - result.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromHours(1))
                    {
                        Database.CommandGiveCredits(command.User.Id, reward);
                        await Database.SetDateTime(command.User.Id, "last_hourly_reward", DateTime.Now);
                        await Messages.CoinsGain(command, reward);
                    }
                    else
                    {
                        await Messages.TooEarly(command, TimeSpan.FromHours(1) - timeSpan);
                    }
                }
                else
                {
                    Database.CommandGiveCredits(command.User.Id, reward);
                    await Database.SetDateTime(command.User.Id, "last_hourly_reward", DateTime.Now);
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