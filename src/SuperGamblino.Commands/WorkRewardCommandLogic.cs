using System;
using System.Threading.Tasks;
using SuperGamblino.Core.GamesObjects;
using SuperGamblino.Infrastructure.Connectors;
using SuperGamblino.Messages;

namespace SuperGamblino.Commands
{
    public class WorkRewardCommandLogic
    {
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public WorkRewardCommandLogic(MessagesHelper messagesHelper, UsersConnector usersConnector)
        {
            _messagesHelper = messagesHelper;
            _usersConnector = usersConnector;
        }

        public async Task<Message> GetWorkReward(ulong userId)
        {
            var user = await _usersConnector.GetUser(userId);
            var lastReward = await _usersConnector.GetDateTime(userId, "last_work_reward");

            var currentJob = Work.GetCurrentJob(user.Level);

            if (lastReward.Successful)
            {
                if (lastReward.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - lastReward.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromHours(currentJob.Cooldown))
                    {
                        await _usersConnector.CommandGiveCredits(userId, currentJob.Reward);
                        await _usersConnector.SetDateTime(userId, "last_work_reward", DateTime.Now);
                        return _messagesHelper.CreditsGain(currentJob.Reward, "WorkReward");
                    }

                    return _messagesHelper.CommandCalledTooEarly(TimeSpan.FromHours(currentJob.Cooldown) - timeSpan,
                        "!work", "WorkReward");
                }

                await _usersConnector.CommandGiveCredits(userId, currentJob.Reward);
                await _usersConnector.SetDateTime(userId, "last_work_reward", DateTime.Now);
                return _messagesHelper.CreditsGain(currentJob.Reward, "WorkReward");
            }

            return _messagesHelper.Error("Some problem with DB occured!");
        }
    }
}