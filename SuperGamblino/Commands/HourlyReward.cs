using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace SuperGamblino.Commands
{
    internal class HourlyReward
    {
        private readonly Database _database;
        private readonly Messages _messages;

        public HourlyReward(Database database, Messages messages)
        {
            _database = database;
            _messages = messages;
        }

        [Command("hourly")]
        [Aliases("get-hourly")]
        [Description("Gives you 20 credits. This command is available once every hour.")]
        public async Task OnExecute(CommandContext command)
        {
            const int reward = 20;
            var result = await _database.GetDateTime(command.User.Id, "last_hourly_reward");
            if (result.Successful)
            {
                if (result.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - result.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromHours(1))
                    {
                        await _database.CommandGiveCredits(command.User.Id, reward);
                        await _database.SetDateTime(command.User.Id, "last_hourly_reward", DateTime.Now);
                        await _messages.CoinsGain(command, reward);
                    }
                    else
                    {
                        await _messages.TooEarly(command, TimeSpan.FromHours(1) - timeSpan);
                    }
                }
                else
                {
                    await _database.CommandGiveCredits(command.User.Id, reward);
                    await _database.SetDateTime(command.User.Id, "last_hourly_reward", DateTime.Now);
                    await _messages.CoinsGain(command, reward);
                }
            }
            else
            {
                await _messages.Error(command, "Some problem with DB occured!");
            }
        }
    }
}