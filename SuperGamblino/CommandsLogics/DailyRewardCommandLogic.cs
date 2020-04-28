using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.CommandsLogics
{
    public class DailyRewardCommandLogic
    {
        private const int Reward = 500;
        private readonly MessagesHelper _messagesHelper;

        private readonly UsersConnector _usersConnector;

        public DailyRewardCommandLogic(UsersConnector usersConnector, MessagesHelper messagesHelper)
        {
            _usersConnector = usersConnector;
            _messagesHelper = messagesHelper;
        }

        public async Task<DiscordEmbed> GetDailyReward(ulong userId)
        {
            var result = await _usersConnector.GetDateTime(userId, "last_daily_reward");
            if (result.Successful)
            {
                if (result.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - result.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromDays(1))
                    {
                        await _usersConnector.CommandGiveCredits(userId, Reward);
                        await _usersConnector.SetDateTime(userId, "last_daily_reward", DateTime.Now);
                        return _messagesHelper.CreditsGain(Reward, "DailyReward");
                    }

                    return _messagesHelper.CommandCalledTooEarly(TimeSpan.FromDays(1) - timeSpan, "!daily",
                        "DailyReward");
                }

                await _usersConnector.CommandGiveCredits(userId, Reward);
                await _usersConnector.SetDateTime(userId, "last_daily_reward", DateTime.Now);
                return _messagesHelper.CreditsGain(Reward, "DailyReward");
            }

            return _messagesHelper.Error("Some problem with DB occured!");
        }
    }
}