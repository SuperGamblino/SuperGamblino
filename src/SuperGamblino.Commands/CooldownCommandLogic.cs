using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SuperGamblino.Core.CommandsObjects;
using SuperGamblino.Infrastructure.Connectors;
using SuperGamblino.Messages;

namespace SuperGamblino.Commands
{
    public class CooldownCommandLogic
    {
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public CooldownCommandLogic(UsersConnector usersConnector, MessagesHelper messagesHelper)
        {
            _usersConnector = usersConnector;
            _messagesHelper = messagesHelper;
        }

        public async Task<Message> GetCooldowns(ulong userId)
        {
            var curLhr = await _usersConnector.GetDateTime(userId, "last_hourly_reward");
            var curLdr = await _usersConnector.GetDateTime(userId, "last_daily_reward");
            var curLwr = await _usersConnector.GetDateTime(userId, "last_work_reward");
            var curLvr = await _usersConnector.GetDateTime(userId, "last_vote_reward");
            var cooldownObjects = new List<CooldownObject>();

            if (curLhr.Successful && curLdr.Successful && curLwr.Successful && curLvr.Successful)
            {
                if (curLhr.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - curLhr.DateTime.Value;
                    cooldownObjects.Add(timeSpan >= TimeSpan.FromHours(1)
                        ? new CooldownObject("Hourly")
                        : new CooldownObject("Hourly", TimeSpan.FromHours(1) - timeSpan));
                }
                else
                {
                    cooldownObjects.Add(new CooldownObject("Hourly"));
                }

                if (curLdr.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - curLdr.DateTime.Value;
                    cooldownObjects.Add(timeSpan >= TimeSpan.FromDays(1)
                        ? new CooldownObject("Daily")
                        : new CooldownObject("Daily", TimeSpan.FromDays(1) - timeSpan));
                }
                else
                {
                    cooldownObjects.Add(new CooldownObject("Daily"));
                }

                if (curLwr.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - curLwr.DateTime.Value;
                    cooldownObjects.Add(timeSpan >= TimeSpan.FromHours(6)
                        ? new CooldownObject("Work")
                        : new CooldownObject("Work", TimeSpan.FromHours(6) - timeSpan));
                }
                else
                {
                    cooldownObjects.Add(new CooldownObject("Work"));
                }

                if (curLvr.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - curLvr.DateTime.Value;
                    cooldownObjects.Add(timeSpan >= TimeSpan.FromHours(12)
                        ? new CooldownObject("Vote")
                        : new CooldownObject("Vote", TimeSpan.FromHours(12) - timeSpan));
                }
                else
                {
                    cooldownObjects.Add(new CooldownObject("Vote"));
                }

                return _messagesHelper.ListCurrentCooldowns(cooldownObjects, "Cooldowns");
            }

            return _messagesHelper.Error("Some problem with DB occured!!!");
        }
    }
}