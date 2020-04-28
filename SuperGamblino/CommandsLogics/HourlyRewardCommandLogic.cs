using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.CommandsLogics
{
    public class HourlyRewardCommandLogic
    {
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public HourlyRewardCommandLogic(UsersConnector usersConnector, MessagesHelper messagesHelper)
        {
            _usersConnector = usersConnector;
            _messagesHelper = messagesHelper;
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
                        return _messagesHelper.CreditsGain(reward, "HourlyReward");
                    }

                    return _messagesHelper.CommandCalledTooEarly(TimeSpan.FromHours(1) - timeSpan, "!hourly",
                        "HourlyReward");
                }

                await _usersConnector.CommandGiveCredits(userId, reward);
                await _usersConnector.SetDateTime(userId, "last_hourly_reward", DateTime.Now);
                return _messagesHelper.CreditsGain(reward, "HourlyReward");
            }

            return _messagesHelper.Error("Some problem with DB occured!");
        }
    }
}