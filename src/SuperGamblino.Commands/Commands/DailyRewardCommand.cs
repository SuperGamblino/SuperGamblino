using System;
using System.Threading.Tasks;
using SuperGamblino.Infrastructure.Connectors;
using SuperGamblino.Messages;

namespace SuperGamblino.Commands.Commands
{
    public class DailyRewardCommand
    {
        private const int Reward = 500;
        private readonly MessagesHelper _messagesHelper;

        private readonly UsersConnector _usersConnector;

        public DailyRewardCommand(UsersConnector usersConnector, MessagesHelper messagesHelper)
        {
            _usersConnector = usersConnector;
            _messagesHelper = messagesHelper;
        }

        public async Task<Message> GetDailyReward(ulong userId)
        {
            var user = await _usersConnector.GetUser(userId);
            if (user.LastDailyReward.HasValue)
            {
                var timeSpan = DateTime.Now - user.LastDailyReward.Value;
                if (timeSpan >= TimeSpan.FromDays(1))
                {
                    user.LastDailyReward = DateTime.Now;
                    user.Credits += Reward;
                    await _usersConnector.UpdateUser(user);
                    return _messagesHelper.CreditsGain(Reward, "DailyReward");
                }

                return _messagesHelper.CommandCalledTooEarly(TimeSpan.FromDays(1) - timeSpan, "!daily",
                    "DailyReward");
            }

            user.LastDailyReward = DateTime.Now;
            user.Credits += Reward;
            await _usersConnector.UpdateUser(user);
            return _messagesHelper.CreditsGain(Reward, "DailyReward");
        }
    }
}