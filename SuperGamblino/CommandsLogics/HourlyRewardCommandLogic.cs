using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.CommandsLogics
{
    public class HourlyRewardCommandLogic
    {
        private readonly Messages _messages;
        private readonly UsersConnector _usersConnector;

        public HourlyRewardCommandLogic(UsersConnector usersConnector, Messages messages)
        {
            _usersConnector = usersConnector;
            _messages = messages;
        }

        public async Task<DiscordEmbed> GetHourlyReward(ulong userId)
        {
            const int reward = 20;
            var result = await _usersConnector.GetDateTime(userId, "last_hourly_reward");
            if (result.Successful)
            {
                if (result.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - result.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromHours(1))
                    {
                        await _usersConnector.CommandGiveCredits(userId, reward);
                        await _usersConnector.SetDateTime(userId, "last_hourly_reward", DateTime.Now);
                        return _messages.CreditsGain(reward, "HourlyReward");
                    }

                    return _messages.CommandCalledTooEarly(TimeSpan.FromHours(1) - timeSpan, "!hourly",
                        "HourlyReward");
                }

                await _usersConnector.CommandGiveCredits(userId, reward);
                await _usersConnector.SetDateTime(userId, "last_hourly_reward", DateTime.Now);
                return _messages.CreditsGain(reward, "HourlyReward");
            }

            return _messages.Error("Some problem with Db occured!");
        }
    }
}