using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.Commands
{
    internal class DailyReward
    {
        private readonly UsersConnector _usersConnector;
        private readonly Messages _messages;

        public DailyReward(Messages messages, UsersConnector usersConnector)
        {
            _messages = messages;
            _usersConnector = usersConnector;
        }

        [Command("daily")]
        [Aliases("get-daily")]
        [Description("Gives you 500 credits. This command is available once every day.")]
        public async Task OnExecute(CommandContext command)
        {
            const int reward = 500;
            var result = await _usersConnector.GetDateTime(command.User.Id, "last_daily_reward");
            if (result.Successful)
            {
                if (result.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - result.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromDays(1))
                    {
                        await _usersConnector.CommandGiveCredits(command.User.Id, reward);
                        await _usersConnector.SetDateTime(command.User.Id, "last_daily_reward", DateTime.Now);
                        await _messages.CoinsGain(command, reward);
                    }
                    else
                    {
                        await _messages.TooEarly(command, TimeSpan.FromDays(1) - timeSpan);
                    }
                }
                else
                {
                    await _usersConnector.CommandGiveCredits(command.User.Id, reward);
                    await _usersConnector.SetDateTime(command.User.Id, "last_daily_reward", DateTime.Now);
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