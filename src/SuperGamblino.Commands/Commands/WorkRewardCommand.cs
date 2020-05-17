using System;
using System.Threading.Tasks;
using SuperGamblino.Core.GamesObjects;
using SuperGamblino.Infrastructure.Connectors;
using SuperGamblino.Messages;

namespace SuperGamblino.Commands.Commands
{
    public class WorkRewardCommand
    {
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public WorkRewardCommand(MessagesHelper messagesHelper, UsersConnector usersConnector)
        {
            _messagesHelper = messagesHelper;
            _usersConnector = usersConnector;
        }

        public async Task<Message> GetWorkReward(ulong userId)
        {
            var user = await _usersConnector.GetUser(userId);
            var currentJob = Work.GetCurrentJob(user.Level);

            if (user.LastWorkReward.HasValue)
            {
                var timeSpan = DateTime.Now - user.LastWorkReward;
                if (timeSpan >= TimeSpan.FromHours(currentJob.Cooldown))
                {
                    user.Credits += currentJob.Reward;
                    user.LastWorkReward = DateTime.Now;
                    await _usersConnector.UpdateUser(user);
                    return _messagesHelper.CreditsGain(currentJob.Reward, "WorkReward");
                }

                return _messagesHelper.CommandCalledTooEarly(TimeSpan.FromHours(currentJob.Cooldown) - timeSpan,
                    "!work", "WorkReward");
            }

            user.Credits += currentJob.Reward;
            user.LastWorkReward = DateTime.Now;
            await _usersConnector.UpdateUser(user);
            return _messagesHelper.CreditsGain(currentJob.Reward, "WorkReward");
        }
    }
}