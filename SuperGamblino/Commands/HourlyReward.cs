using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.Commands
{
    internal class HourlyReward
    {
        private readonly UsersConnector _usersConnector;
        private readonly Messages _messages;

        public HourlyReward(Messages messages, UsersConnector usersConnector)
        {
            _messages = messages;
            _usersConnector = usersConnector;
        }

        [Command("hourly")]
        [Aliases("get-hourly")]
        [Description("Gives you 20 credits. This command is available once every hour.")]
        public async Task OnExecute(CommandContext command)
        {
            throw new NotImplementedException();
            const int reward = 20;
            var result = await _usersConnector.GetDateTime(command.User.Id, "last_hourly_reward");
            if (result.Successful)
            {
                if (result.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - result.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromHours(1))
                    {
                        await _usersConnector.CommandGiveCredits(command.User.Id, reward);
                        await _usersConnector.SetDateTime(command.User.Id, "last_hourly_reward", DateTime.Now);
                        await _messages.CoinsGain(command, reward);
                    }
                    else
                    {
                        await _messages.TooEarly(command, TimeSpan.FromHours(1) - timeSpan);
                    }
                }
                else
                {
                    await _usersConnector.CommandGiveCredits(command.User.Id, reward);
                    await _usersConnector.SetDateTime(command.User.Id, "last_hourly_reward", DateTime.Now);
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