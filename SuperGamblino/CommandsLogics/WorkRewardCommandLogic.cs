using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;

namespace SuperGamblino.CommandsLogics
{
    public class WorkRewardCommandLogic
    {
        private readonly Messages _messages;
        private readonly UsersConnector _usersConnector;

        public WorkRewardCommandLogic(Messages messages, UsersConnector usersConnector)
        {
            _messages = messages;
            _usersConnector = usersConnector;
        }

        public async Task<DiscordEmbed> GetWorkReward(ulong userId)
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
                        return _messages.CreditsGain(currentJob.Reward, "WorkReward");
                    }

                    return _messages.CommandCalledTooEarly(TimeSpan.FromHours(currentJob.Cooldown) - timeSpan,
                        "!work", "WorkReward");
                }

                await _usersConnector.CommandGiveCredits(userId, currentJob.Reward);
                await _usersConnector.SetDateTime(userId, "last_work_reward", DateTime.Now);
                return _messages.CreditsGain(currentJob.Reward, "WorkReward");
            }

            return _messages.Error("Some problem with Db occured!");
        }
    }
}