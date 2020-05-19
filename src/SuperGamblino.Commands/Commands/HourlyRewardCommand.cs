using System;
using System.Threading.Tasks;
using SuperGamblino.Infrastructure.Connectors;
using SuperGamblino.Messages;

namespace SuperGamblino.Commands.Commands
{
    public class HourlyRewardCommand
    {
        const int Reward = 20;
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public HourlyRewardCommand(UsersConnector usersConnector, MessagesHelper messagesHelper)
        {
            _usersConnector = usersConnector;
            _messagesHelper = messagesHelper;
        }

        public async Task<Message> GetHourlyReward(ulong userId)
        {
            var user = await _usersConnector.GetUser(userId);
            if (user.LastHourlyReward.HasValue)
            {
                var timeSpan = DateTime.Now - user.LastHourlyReward.Value;
                if (timeSpan >= TimeSpan.FromDays(1))
                {
                    user.LastHourlyReward = DateTime.Now;
                    user.Credits += Reward;
                    await _usersConnector.UpdateUser(user);
                    return _messagesHelper.CreditsGain(Reward, "HourlyReward");
                }

                return _messagesHelper.CommandCalledTooEarly(TimeSpan.FromDays(1) - timeSpan, "!hourly",
                    "HourlyReward");
            }
            
            user.Credits += Reward;
            user.LastHourlyReward = DateTime.Now;
            await _usersConnector.UpdateUser(user);
            return _messagesHelper.CreditsGain(Reward, "HourlyReward");
        }
    }
}